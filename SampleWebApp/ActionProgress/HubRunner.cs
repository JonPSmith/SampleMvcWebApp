using System;
using GenericServices;
using GenericServices.Actions;
using GenericServices.Logger;
using GenericServices.Services;

namespace SampleWebApp.ActionProgress
{

    public class HubRunner<T> : IActionComms, IHubControl where T : class
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

        /// <summary>
        /// The user data to be handed to the action
        /// </summary>
        private readonly T _dto;

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
        public string ActionId { get; private set; }

        public HubRunner(string actionId, Type actionType, T dto)
        {
            if (ActionHub.LifeTimeScopeProvider == null)
                throw new NullReferenceException("You must set up the static varable HubRunner.LifeTimeScopeProvider before using ActionSetup etc.");

            ActionId = actionId;
            _actionType = actionType;
            _dto = dto;
        }

        /// <summary>
        /// This runs the service while including the actionHub if userConnectionId is set up.
        /// This runs until the action is finished.
        /// </summary>
        /// <param name="actionId">Id of the action. Used for checking and security</param>
        /// <param name="userConnectionId">If null then does not communicate via ActionHub</param>
        /// <param name="hubSendMethods"></param>
        public ProgressMessage RunActionSynchronously(string actionId, string userConnectionId, IActionHubSend hubSendMethods)
        {
            IActionHubDependencyResolver diContainer = null;
            ISuccessOrErrors actionsResult = new SuccessOrErrors();
            UserConnectionId = userConnectionId;
            _hubSendMethods = hubSendMethods;                   //we get a link the the ActionHub for the progress message to use

            if (actionId != ActionId)
            {
                Logger.Error("Failed checks on ActionId when starting action. Security issue?");
                return ProgressMessage.FinishedMessage(true, "Failed checks on ActionId");
            }

            if (userConnectionId != null)
                hubSendMethods.Started(this);

            IActionDefn<T> actionToRun = null;

            try
            {
                //We use DI to resolve the service
                diContainer = ActionHub.LifeTimeScopeProvider();
                //AutofacDependencyResolver.Current.ApplicationContainer.BeginLifetimeScope("httpRequest");
                actionToRun = diContainer.Resolve(_actionType) as IActionDefn<T>;

                if (actionToRun == null)
                {
                    Logger.Error("Could not resolve interface or interface did not resolve to IActionDefn<T>");
                    actionsResult =
                        actionsResult.AddSingleError("The system had a problem finding the service you requested.");
                }
                else
                {
                    // Start the time-consuming operation.
                    Logger.InfoFormat("Started action of type {0}", actionToRun.GetType().Name);
                    actionsResult = actionToRun.DoAction((userConnectionId == null ? null : this), _dto);
                }
            }
            catch (OperationCanceledException)
            {
                //This is how we catch a user cancellation. 
                actionsResult.SetSuccessMessage("Cancelled by user.");  //set as success as cancel is not an error
                CancellationPending = true;                 //We set the flag in case they called the exception by hand
            }
            catch (Exception e)
            {
                Logger.Critical(string.Format("Action {0} had exception.", typeof(T).Name), e);
                actionsResult = actionsResult.AddSingleError("The running action has a system level problem.");
            }
            finally
            {
                if (diContainer != null)
                    diContainer.Dispose();

                var disposable = actionToRun as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }

            if (!actionsResult.IsValid)
            {
                //we need to send over all the error messages to the user
                foreach (var error in actionsResult.Errors)
                {
                    Logger.WarnFormat("Action error: {0}", error.ErrorMessage);
                    hubSendMethods.Progress(this, 100,
                                                    new ProgressMessage(ProgressMessageTypes.Error, error.ErrorMessage));
                }
            }

            ProgressMessage actionStatus;
            if (CancellationPending)
            {
                Logger.InfoFormat("Cancelled by user.");
                actionStatus = ProgressMessage.CancelledMessage("Cancelled by user.");
            }
            else
            {
                Logger.InfoFormat("Finished: {0}", actionsResult);
                actionStatus = ProgressMessage.FinishedMessage(!actionsResult.IsValid, actionsResult.ToString());
            }

            if (userConnectionId != null)
                hubSendMethods.Stopped(this, actionStatus);

            //Always remove at end
            ActionHub.RemoveActionRunner(actionId);

            return actionStatus;
        }

        /// <summary>
        /// This sets the pendingCancel flag on a action
        /// </summary>
        /// <param name="actionId"></param>
        /// <returns>returns true if there was a problem. This allows the caller to sort things out</returns>
        public bool CancelRunningAction(string actionId)
        {
            if (actionId != ActionId)
            {
                Logger.Error("Failed checks on ActionId when being cancelled. Security issue?");
                return true;
            }

            CancellationPending = true;
            return false;
        }

    }
}