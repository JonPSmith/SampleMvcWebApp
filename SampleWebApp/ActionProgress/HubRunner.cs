using System;
using System.Threading.Tasks;
using GenericServices;
using GenericServices.Actions;
using GenericServices.Core;
using GenericServices.Logger;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;

namespace SampleWebApp.ActionProgress
{

    public class HubRunner<TActionOut, TActionIn> : IActionComms, IHubControl where TActionIn : class
    {
        private static readonly IGenericLogger Logger;

        static HubRunner()
        {
            Logger = GenericLoggerFactory.GetLogger("HubRunner");
        }

        /// <summary>
        /// This is used by progress message to send the message via the 
        /// </summary>
        private IActionHubSend _hubSendMethods;

        /// <summary>
        /// This holds the type of services that need to be created by dependency injection
        /// </summary>
        private readonly Type _actionType;

        private readonly bool _isAsync;

        /// <summary>
        /// The user data to be handed to the action
        /// </summary>
        private readonly TActionIn _dto;

        //--------------------------------------------------
        //IAction Comms items

        /// <summary>
        /// This throws a OperationCanceledException if the task has a CancellationPending
        /// </summary>
        public void ThrowExceptionIfCancelPending()
        {
            if (CancellationPending)
                throw new OperationCanceledException();
        }

        /// <summary>
        /// This sends a status update to the user from the running action
        /// </summary>
        /// <param name="percentageDone">goes from 0 to 100</param>
        /// <param name="message">message, with message type in. Can be null for no message</param>
        public void ReportProgress(int percentageDone, ProgressMessage message)
        {
            if (UserConnectionId != null)
                _hubSendMethods.Progress(this, percentageDone, message);
        }

        //-------------------------------------------------------------------------------------

        /// <summary>
        /// This is true if the user has asked for the action to cancel
        /// </summary>
        public bool CancellationPending { get; internal set; }



        //---------------------------------
        //public items

        /// <summary>
        /// This is the connnectionId to talk directly to the user on a specific connection
        /// </summary>
        public string UserConnectionId { get; private set; }

        /// <summary>
        /// This is the action Id of the created action. Used for for saving in dictionary and security checks
        /// </summary>
        public string ActionGuid { get; private set; }

        public HubRunner(string actionGuid, Type actionType, TActionIn dto, bool isAsync)
        {
            if (ActionHub.LifeTimeScopeProvider == null)
                throw new NullReferenceException("You must set up the static varable HubRunner.LifeTimeScopeProvider before using ActionSetup etc.");

            ActionGuid = actionGuid;
            _actionType = actionType;
            _dto = dto;
            _isAsync = isAsync;
        }

        /// <summary>
        /// This runs the service while including the actionHub if userConnectionId is set up.
        /// This runs until the action is finished.
        /// </summary>
        /// <param name="actionGuid">Id of the action. Used for checking and security</param>
        /// <param name="userConnectionId">If null then does not communicate via ActionHub</param>
        /// <param name="hubSendMethods"></param>
        public async Task<ProgressMessage> RunActionAsync(string actionGuid, string userConnectionId, IActionHubSend hubSendMethods)
        {
            IActionHubDependencyResolver diContainer = null;
            ISuccessOrErrors status = new SuccessOrErrors();
            UserConnectionId = userConnectionId;
            _hubSendMethods = hubSendMethods;                   //we get a link the the ActionHub for the progress message to use

            if (actionGuid != ActionGuid)
            {
                Logger.Error("Failed checks on ActionGuid when starting action. Security issue?");
                return ProgressMessage.FinishedMessage(true, "Failed checks on ActionGuid");
            }

            dynamic actionToRun = null;

            try
            {
                //We use DI to resolve the service
                diContainer = ActionHub.LifeTimeScopeProvider();
                actionToRun = diContainer.Resolve(_actionType) as IActionBase;

                if (actionToRun == null)
                {
                    Logger.ErrorFormat("The interface '{0}' did not resolve to IActionBase", _actionType.Name);
                    status =
                        status.AddSingleError("The system could not find the service you requested.");

                }
                else
                {                 
                    // Start the time-consuming operation.
                    hubSendMethods.Started(this, actionToRun.ActionConfig.ToString());

                    Logger.InfoFormat("Started action of type {0}", actionToRun.GetType().Name);
                    status = _isAsync
                        ? await actionToRun.DoActionAsync(this, _dto).ConfigureAwait(false)
                        : actionToRun.DoAction(this, _dto);

                    if (CheckIfSumbitChangesShouldBeCalled(status, actionToRun, _dto))
                        status = await CallSubmitChanges(status, diContainer);
                }
            }
            catch (OperationCanceledException)
            {
                //This is how we catch a user cancellation. 
                status.SetSuccessMessage("Cancelled by user."); //set as success as cancel is not an error
                CancellationPending = true; //We set the flag in case they called the exception by hand
            }
            catch (RuntimeBinderException ex)
            {
                Logger.Critical(string.Format("The interface '{0}' did not resolve to IActionDefn<{1},{2}>",
                    _actionType.Name, typeof(TActionOut).Name, typeof(TActionIn).Name), ex);
                status =
                    status.AddSingleError("The system had a problem finding the service you requested.");
            }
            catch (Exception e)
            {
                Logger.Critical(string.Format("Action of interface type '{0}' had an exception.", _actionType.Name), e);
                status = status.AddSingleError("The running action has a system level problem.");
            }
            finally
            {
                if (diContainer != null)
                    diContainer.Dispose();

                var disposable = actionToRun as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }

            var finishMessage = SendStoppedMessage(actionGuid, hubSendMethods, status);

            return finishMessage;
        }

        /// <summary>
        /// This sets the pendingCancel flag on a action
        /// </summary>
        /// <param name="actionGuid"></param>
        /// <returns>returns true if there was a problem. This allows the caller to sort things out</returns>
        public bool CancelRunningAction(string actionGuid)
        {
            if (actionGuid != ActionGuid)
            {
                Logger.Error("Failed checks on ActionGuid when being cancelled. Security issue?");
                return true;
            }

            CancellationPending = true;
            return false;
        }

        //--------------------------------------------------------
        //private methods

        /// <summary>
        /// This handles sending the closing message to the JavaScript client side
        /// </summary>
        /// <param name="actionGuid"></param>
        /// <param name="hubSendMethods"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private ProgressMessage SendStoppedMessage(string actionGuid, IActionHubSend hubSendMethods, ISuccessOrErrors status)
        {
            if (!status.IsValid)
            {
                //we need to send over all the error messages to the user
                foreach (var error in status.Errors)
                {
                    Logger.WarnFormat("Action error: {0}", error.ErrorMessage);
                    hubSendMethods.Progress(this, 100,
                        new ProgressMessage(ProgressMessageTypes.Error, error.ErrorMessage));
                }
            }

            ProgressMessage finishMessage;
            string jsonToSend = null;
            if (CancellationPending)
            {
                Logger.InfoFormat("Cancelled by user.");
                finishMessage = ProgressMessage.CancelledMessage("Cancelled by user.");
            }
            else if (!status.IsValid)
            {
                Logger.InfoFormat("Finished with errors: {0}", status);
                finishMessage = ProgressMessage.FinishedMessage(true, status.ToString());
            }
            else
            {
                Logger.InfoFormat("Finished OK: {0}", status);
                finishMessage = ProgressMessage.FinishedMessage(false, status.ToString());

                jsonToSend = ExtractAndDecodeResultFromStatus<TActionOut>(status);
            }

            hubSendMethods.Stopped(this, finishMessage, jsonToSend);

            //Always remove at end
            ActionHub.RemoveActionRunner(actionGuid);
            return finishMessage;
        }

        /// <summary>
        /// This will extract a result from the final status and encode it as json.
        /// We use the Newtonsoft Json converter as it allows attributes for handling enums etc.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="status"></param>
        /// <returns></returns>
        private string ExtractAndDecodeResultFromStatus<T>(ISuccessOrErrors status)
        {
            var resultStatus = status as ISuccessOrErrors<T>;
            if (resultStatus == null)
                return null;                //Shouldn't happen, but safe way of handling this

            return JsonConvert.SerializeObject(resultStatus.Result);
        }

        //----------------------
        //method for handling the writing out of data

        private static bool CheckIfSumbitChangesShouldBeCalled(ISuccessOrErrors status, IActionBase action, TActionIn actionData)
        {
            if (!status.IsValid || !action.SubmitChangesOnSuccess) return false;     //nothing to do

            if (ShouldStopAsWarningsMatter(status.HasWarnings, actionData))
            {
                //There were warnings and we are asked to not write to the database
                status.SetSuccessMessage("{0}... but NOT written to database as warnings.", status.SuccessMessage);
                return false;
            }

            return true;
        }


        private static bool ShouldStopAsWarningsMatter<T>(bool hasWarnings, T classToCheck)
        {
            if (!hasWarnings) return false;
            var flagClass = classToCheck as ICheckIfWarnings;
            return (flagClass != null && !flagClass.WriteEvenIfWarning);
        }


        private static async Task<ISuccessOrErrors> CallSubmitChanges(ISuccessOrErrors status, IActionHubDependencyResolver diContainer)
        {

            //we now need to save the changes to the database
            var db = diContainer.Resolve(typeof(IDbContextWithValidation)) as IDbContextWithValidation;
            if (db == null)
                throw new NullReferenceException("IDbContextWithValidation must resolve via DI for HubRunner db to work.");
            
            var dataStatus = await db.SaveChangesWithValidationAsync();
            return dataStatus.IsValid
                ? status.SetSuccessMessage("{0}... and written to database.", status.SuccessMessage)
                : dataStatus;
        }


    }
}