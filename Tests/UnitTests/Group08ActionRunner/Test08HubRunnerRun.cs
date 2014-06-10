using System.Linq;
using Autofac;
using GenericServices;
using GenericServices.Actions;
using GenericServices.Logger;
using log4net.Appender;
using log4net.Config;
using NUnit.Framework;
using SampleWebApp.ActionProgress;
using ServiceLayer.TestActionService;
using ServiceLayer.TestActionService.Concrete;
using Tests.Helpers;
using Tests.MocksAndFakes;

namespace Tests.UnitTests.Group08ActionRunner
{
    class Test08HubRunnerRun
    {
        private MemoryAppender _log4NetMemoryLog;
        private MockActionHubSend _mockHub;

        private CommsTestAction _action;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {

            _log4NetMemoryLog = new MemoryAppender();
            BasicConfigurator.Configure(_log4NetMemoryLog);
            GenericLoggerFactory.SetLoggerMethod = name => new Log4NetGenericLogger(name);
        }

        [SetUp]
        public void Setup()
        {
            _log4NetMemoryLog.Clear();
            _mockHub = new MockActionHubSend();

            _action = new CommsTestAction();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(_action).As<ICommsTestAction>();
            var container = builder.Build();
            ActionHub.LifeTimeScopeProvider = () => new AutoFacActionHubResolver(container);
        }

        [Test]
        public void Test01RunCommsTestActionOk()
        {
            //SETUP
            var data = new CommsTestActionData
            {
                SecondsBetweenIterations = 0
            };
            var hr = new HubRunner<CommsTestActionData>("aaa", typeof(ICommsTestAction), data);

            //ATTEMPT
            var lastMessage = hr.RunActionSynchronously("aaa", "123", _mockHub);

            //VERIFY
            lastMessage.MessageType.ShouldEqual(ProgressMessageTypes.Finished);
        }

        [Test]
        public void Check01CheckProgressNumbersOk()
        {

            //SETUP

            var data = new CommsTestActionData
            {
                Mode = TestServiceModes.RunSuccessfully,
                NumIterations = 2,
                SecondsBetweenIterations = 0
            };
            var hr = new HubRunner<CommsTestActionData>("aaa", typeof(ICommsTestAction), data);

            //ATTEMPT
            hr.RunActionSynchronously("aaa", "123", _mockHub);

            //VERIFY
            _mockHub.ProgressWithMessages.Count.ShouldEqual(data.NumIterations + 2);
            string.Join(",", _mockHub.PercentagesOnly).ShouldEqual("-1,0,50,100,101");
        }

        [Test]
        public void Check02RunSuccessfullyDifferentProgressBoundsOk()
        {

            //SETUP
            _action.LowerBound = 25;
            _action.UpperBound = 75;
            var data = new CommsTestActionData
            {
                Mode = TestServiceModes.RunSuccessfully,
                NumIterations = 2,
                SecondsBetweenIterations = 0
            };
            var hr = new HubRunner<CommsTestActionData>("aaa", typeof(ICommsTestAction), data);

            //ATTEMPT
            hr.RunActionSynchronously("aaa", "123", _mockHub);

            //VERIFY
            _mockHub.ProgressWithMessages.Count.ShouldEqual(data.NumIterations + 2);
            string.Join(",", _mockHub.PercentagesOnly).ShouldEqual("-1,25,50,75,101");
        }

        [Test]
        public void Check05RunButOutputErrorsOk()
        {
            //SETUP
            var data = new CommsTestActionData
            {
                Mode = TestServiceModes.RunButOutputErrors,
                NumIterations = 2,
                SecondsBetweenIterations = 0
            };
            var hr = new HubRunner<CommsTestActionData>("aaa", typeof(ICommsTestAction), data);

            //ATTEMPT
            hr.RunActionSynchronously("aaa", "123", _mockHub);

            //VERIFY
            _mockHub.ProgressWithMessages.Count.ShouldEqual(data.NumIterations + 2);
            _mockHub.ProgressWithMessages.Any(x => x.OptionalMessage.MessageType == ProgressMessageTypes.Info).ShouldEqual(true);
            _mockHub.ProgressWithMessages.Any(x => x.OptionalMessage.MessageType == ProgressMessageTypes.Error).ShouldEqual(true);
        }

        [Test]
        public void Check06RunButOutputErrorsAndFailOk()
        {

            //SETUP
            var data = new CommsTestActionData
            {
                Mode = TestServiceModes.RunButOutputErrors,
                NumIterations = 2,
                SecondsBetweenIterations = 0,
                NumErrorsToExitWith = 1
            };
            var hr = new HubRunner<CommsTestActionData>("aaa", typeof(ICommsTestAction), data);

            //ATTEMPT
            var finalMess = hr.RunActionSynchronously("aaa", "123", _mockHub);

            //VERIFY
            finalMess.MessageType.ShouldEqual(ProgressMessageTypes.Failed);

        }

        [Test]
        public void Check10ThrowExceptionOnStartOk()
        {

            //SETUP
            var data = new CommsTestActionData
            {
                Mode = TestServiceModes.ThrowExceptionOnStart,
                NumIterations = 2,
                SecondsBetweenIterations = 0,
            };
            var hr = new HubRunner<CommsTestActionData>("aaa", typeof(ICommsTestAction), data);

            //ATTEMPT
            var finalMess = hr.RunActionSynchronously("aaa", "123", _mockHub);

            //VERIFY
            finalMess.MessageText.ShouldEqual("Failed with 1 errors");
            finalMess.MessageType.ShouldEqual(ProgressMessageTypes.Failed);
            _mockHub.ProgressWithMessages.Count.ShouldEqual(2);
            _mockHub.ProgressWithMessages[0].OptionalMessage.MessageText.ShouldEqual("The running action has a system level problem.");
        }

        [Test]
        public void Check11ThrowExceptionHalfWayThroughOk()
        {

            //SETUP
            var data = new CommsTestActionData
            {
                Mode = TestServiceModes.ThrowExceptionHalfWayThrough,
                NumIterations = 2,
                SecondsBetweenIterations = 0,
            };
            var hr = new HubRunner<CommsTestActionData>("aaa", typeof(ICommsTestAction), data);

            //ATTEMPT
            var finalMess = hr.RunActionSynchronously("aaa", "123", _mockHub);

            //VERIFY
            finalMess.MessageText.ShouldEqual("Failed with 1 errors");
            finalMess.MessageType.ShouldEqual(ProgressMessageTypes.Failed);
            _mockHub.ProgressWithMessages.Count.ShouldEqual(4);
            _mockHub.ProgressWithMessages[2].OptionalMessage.MessageText.ShouldEqual("The running action has a system level problem.");
        }

        [Test]
        public void Check15CancelAtStartOk()
        {

            //SETUP
            var data = new CommsTestActionData
            {
                Mode = TestServiceModes.RunSuccessfully,
                NumIterations = 2,
                SecondsBetweenIterations = 0,
            };
            var hr = new HubRunner<CommsTestActionData>("aaa", typeof(ICommsTestAction), data);
            
            //ATTEMPT
            hr.CancellationPending = true;
            var finalMess = hr.RunActionSynchronously("aaa", "123", _mockHub);

            //VERIFY
            finalMess.MessageText.ShouldEqual("Cancelled by user.");
            finalMess.MessageType.ShouldEqual(ProgressMessageTypes.Cancelled);
            _mockHub.ProgressWithMessages.Count.ShouldEqual(2);
            _mockHub.ProgressWithMessages.Last().OptionalMessage.MessageType.ShouldEqual(ProgressMessageTypes.Cancelled);
        }

        [Test]
        public void Check20DisposeWasCalledOk()
        {

            //SETUP
            //SETUP
            var data = new CommsTestActionData
            {
                Mode = TestServiceModes.RunSuccessfully,
                NumIterations = 2,
                SecondsBetweenIterations = 0,
            };
            var hr = new HubRunner<CommsTestActionData>("aaa", typeof(ICommsTestAction), data);

            //ATTEMPT
            hr.RunActionSynchronously("aaa", "123", _mockHub);

            //VERIFY
            _action.DisposeWasCalled.ShouldEqual(true);
        }



    }
}
