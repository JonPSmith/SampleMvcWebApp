using GenericServices.Actions;

namespace SampleWebApp.ActionProgress
{
    public interface IActionHubSend
    {
        /// <summary>
        /// This is returned to client when the action is started. It confirms to the user that all is OK
        /// </summary>
        /// <param name="actionRunner"></param>
        /// <param name="actionConfig">a string of flags on what options the action supports</param>
        void Started(IHubControl actionRunner, string actionConfig);

        /// <summary>
        /// This sents progress reports to the client with an optional message
        /// </summary>
        /// <param name="actionRunner"></param>
        /// <param name="percentageDone">should run between 0 and 100. If you don't know how long then need another way of doing this</param>
        /// <param name="message">optional message</param>
        void Progress(IHubControl actionRunner, int percentageDone, ProgressMessage message);

        /// <summary>
        /// This is called when the action has stopped, either because it has finished, had an error or its been cancelled.
        /// </summary>
        /// <param name="actionRunner"></param>
        /// <param name="finalMessage">The finalMessage must be present as the MessageType property carries the type of stop</param>
        /// <param name="jsonToSend">This is a json string to send at the end of the process. Will be null if errors or not supplied.</param>
        void Stopped(IHubControl actionRunner, ProgressMessage finalMessage, string jsonToSend);

    }
}
