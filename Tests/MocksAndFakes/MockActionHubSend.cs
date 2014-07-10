using System.Collections.Generic;
using System.Linq;
using GenericServices.ActionComms;
using SampleWebApp.ActionProgress;

namespace Tests.MocksAndFakes
{
    public class MockActionHubSend : IActionHubSend
    {

        private readonly List<ProgressWithOptionalMessage> _logOfCalls = new List<ProgressWithOptionalMessage>();

        public string ActionConfigFlags { get; private set; }

        public IReadOnlyList<ProgressWithOptionalMessage> AllProgressAndMessages { get {  return _logOfCalls.AsReadOnly();} }

        public IReadOnlyList<ProgressWithOptionalMessage> ProgressWithMessages { get { return _logOfCalls.Where(x => x.OptionalMessage != null).ToList().AsReadOnly(); } }

        public IEnumerable<int> PercentagesOnly { get { return _logOfCalls.Select(x => x.PercentageDone); } }

        public object FinalJsonData { get; private set; }

        public void Started(IHubControl actionRunner, string actionConfig)
        {
            ActionConfigFlags = actionConfig;
            _logOfCalls.Add(new ProgressWithOptionalMessage(-1, null));
        }

        public void Progress(IHubControl actionRunner, int percentageDone, ProgressMessage message)
        {
            _logOfCalls.Add(new ProgressWithOptionalMessage(percentageDone, message));
        }

        public void Stopped(IHubControl actionRunner, ProgressMessage finalMessage, object jsonToSend)
        {
            _logOfCalls.Add(new ProgressWithOptionalMessage(101, finalMessage));
            FinalJsonData = jsonToSend;
        }
    }
}
