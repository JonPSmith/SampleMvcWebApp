using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using GenericServices.Actions;
using GenericServices.Logger;
using Microsoft.AspNet.SignalR;

namespace SampleWebApp.ActionProgress
{
    public delegate IActionHubDependencyResolver NewLifeTimeScope();

    public class ActionHub : Hub, IActionHubSend
    {
        private static readonly IGenericLogger Logger;

        static ActionHub()
        {
            Logger = GenericServices.GenericLoggerFactory.GetLogger("ActionHub");
        }

        private static readonly ConcurrentDictionary<string, IHubControl> AllActionsRunning = new ConcurrentDictionary<string, IHubControl>();

        //--------------------------------------------------
        //public static for getting lifetime scoped dependecy resolver

        /// <summary>
        /// This should be set to the method to create a new lifetime scoped dependency injector
        /// </summary>
        public static NewLifeTimeScope LifeTimeScopeProvider { get; set; }

        #region methods called by JavaScript

        /// <summary>
        /// This is called by JavaScript to start a action which has already been set up.
        /// It calls the 'Started' method to inform the called when the action is started successfully,
        /// of calls 'Failed' if there was a problem.
        /// </summary>
        /// <param name="actionId">The actionId is used to look for a Action instance to run</param>
        public void StartAction(string actionId)
        {
            var actionRunner = GetActionRunner(actionId);

            if (actionRunner != null)
                //If this succeeds then this Hub is paused for the duration of the action
                actionRunner.RunActionSynchronously(actionId, Context.ConnectionId, this);
        }

        /// <summary>
        /// This is called by JavaScript to cancels a running action
        /// It calls the 'Cancelled' method to inform the called when the action is cancelled successfully,
        /// of calls 'Failed' if there was a problem.
        /// </summary>
        /// <param name="actionId">The actionId is used to look for a Action instance</param>
        public void CancelAction(string actionId)
        {
            var actionRunner = GetActionRunner(actionId);

            if (actionRunner == null || actionRunner.CancelRunningAction(actionId)) 
                //It wasn't there or wasn't running so we send a cancelled message straght away to stop a potential hangup
                Stopped(actionRunner, ProgressMessage.CancelledMessage("Action had already finished (or there was a problem)."));
        }

        /// <summary>
        /// This is called by JavaScript at the end of the action
        /// </summary>
        /// <param name="actionId">The actionId is used to look for a Action instance to run</param>
        public void EndAction(string actionId)
        {
            //not now used, but left in
        }

        #endregion


        #region methods to send information to JavaScript

        /// <summary>
        /// This is returned to client when the action is started. It confirms to the user that all is OK
        /// </summary>
        /// <param name="actionRunner"></param>
        public void Started(IHubControl actionRunner)
        {
            Clients.Client(actionRunner.UserConnectionId).Started(actionRunner.ActionId);
        }

        /// <summary>
        /// This sents progress reports to the client with an optional message
        /// </summary>
        /// <param name="actionRunner"></param>
        /// <param name="percentageDone">should run between 0 and 100. If you don't know how long then need another way of doing this</param>
        /// <param name="message">optional message</param>
        public void Progress(IHubControl actionRunner, int percentageDone, ProgressMessage message)
        {
            Clients.Client(actionRunner.UserConnectionId).Progress(actionRunner.ActionId, percentageDone, message);
        }

        /// <summary>
        /// This is called when the action has stopped, either because it has finished, had an error or its been cancelled.
        /// </summary>
        /// <param name="actionRunner"></param>
        /// <param name="finalMessage">The finalMessage must be present as the MessageType property carries the type of stop</param>
        public void Stopped(IHubControl actionRunner, ProgressMessage finalMessage)
        {
            Clients.Client(actionRunner.UserConnectionId).Stopped(actionRunner.ActionId, finalMessage);
        }

        #endregion


        //--------------------------------------------------------

        /// <summary>
        /// It is possible the user may abort a job by moving away to another page. In this case
        /// we want to a) cancel their running action and b) remove that entry from the directory
        /// </summary>
        /// <returns></returns>
        public override Task OnDisconnected()
        {
            //we need to be careful here as things are async. The action may be removed by other means in parallel

            var actionIdsOfActionsInDict =
                AllActionsRunning.Values.Where(x => x.UserConnectionId == Context.ConnectionId)
                               .Select(x => x.ActionId)
                               .ToList();

            if (actionIdsOfActionsInDict.Count > 1)
                //There should have only been one action being run by user
                Logger.Error("There was more than one entry in the AllRunningActions which had the user connection. Software error.");

            foreach (var actionId in actionIdsOfActionsInDict)
            {
                IHubControl actionRunner;
                AllActionsRunning.TryGetValue(actionId, out actionRunner);        //due to async nature of actioning it could have been removed elsewhere
                if (actionRunner != null)
                {
                    actionRunner.CancelRunningAction(actionId);                   //make sure its cancelled
                    AllActionsRunning.TryRemove(actionId, out actionRunner);      //try to remove, but ok if it isn't there (due to async behaviour)
                }
            }

            return base.OnDisconnected();
        }


        //--------------------------------------------------------
        //public static helpers

        public static void SetActionRunner(IHubControl actionRunner)
        {
            AllActionsRunning[actionRunner.ActionId] = actionRunner;
        }

        public static void RemoveActionRunner(string actionId)
        {
            IHubControl oldValue;
            if (!AllActionsRunning.TryRemove(actionId, out oldValue))
                Logger.Warn("Attempted to remove actionId from dictionary but wasn't there. Could be concurrency issue.");
        }

        //--------------------------------------------------------
        //private helper

        private static IHubControl GetActionRunner(string actionId)
        {
            return AllActionsRunning[actionId];
        }
    }
}