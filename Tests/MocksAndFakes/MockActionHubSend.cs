using System.Collections.Generic;
using System.Linq;
using GenericServices.Actions;
using SampleWebApp.ActionProgress;

namespace Tests.MocksAndFakes
{
    public class MockActionHubSend : IActionHubSend
    {

        private readonly List<ProgressWithOptionalMessage> _logOFCalls = new List<ProgressWithOptionalMessage>();

        public IReadOnlyList<ProgressWithOptionalMessage> AllProgressAndMessages { get {  return _logOFCalls.AsReadOnly();} }

        public IReadOnlyList<ProgressWithOptionalMessage> ProgressWithMessages { get { return _logOFCalls.Where(x => x.OptionalMessage != null).ToList().AsReadOnly(); } }

        public IReadOnlyList<int> PercentagesOnly { get { return _logOFCalls.Select(x => x.PercentageDone).ToList(); } }

        public void Started(IHubControl actionRunner)
        {
            _logOFCalls.Add(new ProgressWithOptionalMessage(-1, null));
        }

        public void Progress(IHubControl actionRunner, int percentageDone, ProgressMessage message)
        {
            _logOFCalls.Add(new ProgressWithOptionalMessage(percentageDone, message));
        }

        public void Stopped(IHubControl actionRunner, ProgressMessage finalMessage)
        {
            _logOFCalls.Add(new ProgressWithOptionalMessage(101, finalMessage));
        }
    }
}
