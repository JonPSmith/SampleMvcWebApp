using System.Threading.Tasks;
using GenericServices.Actions;

namespace SampleWebApp.ActionProgress
{
    public interface IHubControl
    {
        /// <summary>
        /// This is the guid string of the created Action. Used for security and error checking
        /// </summary>
        string ActionGuid { get; }

        /// <summary>
        /// This is the connnectionId to talk directly to the user on a specific connection
        /// </summary>
        string UserConnectionId { get; }

        /// <summary>
        /// This runs the service as an async Task
        /// </summary>
        /// <param name="actionGuid"></param>
        /// <param name="userConnectionId"></param>
        /// <param name="hubSendMethods"></param>
        /// <returns>Final message, which says if there was a problem</returns>
        Task<ProgressMessage> RunActionAsync(string actionGuid, string userConnectionId, IActionHubSend hubSendMethods);

        /// <summary>
        /// This will cancel a running action
        /// </summary>
        /// <param name="actionGuid"></param>
        /// <returns>true if task was cancelled, or false if task wasn't running</returns>
        bool CancelRunningAction(string actionGuid);
    }
}
