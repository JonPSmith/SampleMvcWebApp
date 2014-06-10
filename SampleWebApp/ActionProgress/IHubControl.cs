using GenericServices.Actions;

namespace SampleWebApp.ActionProgress
{
    public interface IHubControl
    {
        /// <summary>
        /// This is the task Id of the created task. Used for security and error checking
        /// </summary>
        string ActionId { get; }

        /// <summary>
        /// This is the connnectionId to talk directly to the user on a specific connection
        /// </summary>
        string UserConnectionId { get; }

        /// <summary>
        /// This runs the service in the background.
        /// </summary>
        /// <param name="actionId"></param>
        /// <param name="userConnectionId"></param>
        /// <param name="hubSendMethods"></param>
        /// <returns>Final message, which says if there was a problem</returns>
        ProgressMessage RunActionSynchronously(string actionId, string userConnectionId, IActionHubSend hubSendMethods);

        /// <summary>
        /// This will cancel a running action
        /// </summary>
        /// <param name="actionId"></param>
        /// <returns>true if task was cancelled, or false if task wasn't running</returns>
        bool CancelRunningAction(string actionId);
    }
}
