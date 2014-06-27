using System.Linq;
using Autofac;
using DataLayer.DataClasses.Concrete;
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

        private CommsTestActionNormal _action;
        private DummyIDbContextWithValidation _dummyDb;

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
            _dummyDb = new DummyIDbContextWithValidation();

            _action = new CommsTestActionNormal();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(_action).As<ICommsTestActionNormal>();
            builder.RegisterInstance(_dummyDb).As<IDbContextWithValidation>();
            builder.RegisterType<EmptyTestAction>().As<IEmptyTestAction>();
            builder.RegisterType<EmptyTestActionWithSubmit>().As<IEmptyTestActionWithSubmit>();
            var container = builder.Build();
            ActionHub.LifeTimeScopeProvider = () => new AutoFacActionHubResolver(container);
        }

        [Test]
        public async void Check01RunCommsTestActionOk()
        {
            //SETUP
            var data = new CommsTestActionData
            {
                SecondsBetweenIterations = 0
            };
            var hr = new HubRunner<int,CommsTestActionData>("aaa", typeof(ICommsTestActionNormal), data, false);

            //ATTEMPT
            var lastMessage = await hr.RunActionAsync("aaa", "123", _mockHub);

            //VERIFY
            lastMessage.MessageType.ShouldEqual(ProgressMessageTypes.Finished);
            lastMessage.MessageText.ShouldStartWith("Have completed the action in ");
        }

        [Test]
        public async void Check02CheckProgressNumbersOk()
        {

            //SETUP

            var data = new CommsTestActionData
            {
                Mode = TestServiceModes.RunSuccessfully,
                NumIterations = 2,
                SecondsBetweenIterations = 0
            };
            var hr = new HubRunner<int,CommsTestActionData>("aaa", typeof(ICommsTestActionNormal), data, false);

            //ATTEMPT
            await hr.RunActionAsync("aaa", "123", _mockHub);

            //VERIFY
            _mockHub.ProgressWithMessages.Count.ShouldEqual(data.NumIterations + 2);
            string.Join(",", _mockHub.PercentagesOnly).ShouldEqual("-1,0,33,66,101");
        }

        [Test]
        public async void Check03CheckFinalJsonOutOk()
        {

            //SETUP

            var data = new CommsTestActionData
            {
                Mode = TestServiceModes.RunSuccessfully,
                NumIterations = 2,
                SecondsBetweenIterations = 0
            };
            var hr = new HubRunner<int, CommsTestActionData>("aaa", typeof(ICommsTestActionNormal), data, false);

            //ATTEMPT
            await hr.RunActionAsync("aaa", "123", _mockHub);

            //VERIFY
            _mockHub.FinalJsonData.ShouldEqual(data.NumIterations.ToString());
        }

        [Test]
        public async void Check04RunSuccessfullyDifferentProgressBoundsOk()
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
            var hr = new HubRunner<int,CommsTestActionData>("aaa", typeof(ICommsTestActionNormal), data, false);

            //ATTEMPT
            await hr.RunActionAsync("aaa", "123", _mockHub);

            //VERIFY
            _mockHub.ProgressWithMessages.Count.ShouldEqual(data.NumIterations + 2);
            string.Join(",", _mockHub.PercentagesOnly).ShouldEqual("-1,25,41,58,101");
        }

        [Test]
        public async void Check05RunButOutputErrorsOk()
        {
            //SETUP
            var data = new CommsTestActionData
            {
                Mode = TestServiceModes.RunButOutputErrors,
                NumIterations = 2,
                SecondsBetweenIterations = 0
            };
            var hr = new HubRunner<int,CommsTestActionData>("aaa", typeof(ICommsTestActionNormal), data, false);

            //ATTEMPT
            await hr.RunActionAsync("aaa", "123", _mockHub);

            //VERIFY
            _mockHub.ProgressWithMessages.Count.ShouldEqual(data.NumIterations + 2);
            _mockHub.ProgressWithMessages.Any(x => x.OptionalMessage.MessageType == ProgressMessageTypes.Info).ShouldEqual(true);
            _mockHub.ProgressWithMessages.Any(x => x.OptionalMessage.MessageType == ProgressMessageTypes.Error).ShouldEqual(true);
        }

        [Test]
        public async void Check06RunButOutputErrorsAndFailOk()
        {

            //SETUP
            var data = new CommsTestActionData
            {
                Mode = TestServiceModes.RunButOutputErrors,
                NumIterations = 2,
                SecondsBetweenIterations = 0,
                NumErrorsToExitWith = 1
            };
            var hr = new HubRunner<int,CommsTestActionData>("aaa", typeof(ICommsTestActionNormal), data, false);

            //ATTEMPT
            var finalMess = await hr.RunActionAsync("aaa", "123", _mockHub);

            //VERIFY
            finalMess.MessageType.ShouldEqual(ProgressMessageTypes.Failed);
            _mockHub.FinalJsonData.ShouldEqual(null);
        }

        [Test]
        public async void Check10ThrowExceptionOnStartOk()
        {

            //SETUP
            var data = new CommsTestActionData
            {
                Mode = TestServiceModes.ThrowExceptionOnStart,
                NumIterations = 2,
                SecondsBetweenIterations = 0,
            };
            var hr = new HubRunner<int,CommsTestActionData>("aaa", typeof(ICommsTestActionNormal), data, false);

            //ATTEMPT
            var finalMess = await hr.RunActionAsync("aaa", "123", _mockHub);

            //VERIFY
            finalMess.MessageText.ShouldEqual("Failed with 1 error");
            finalMess.MessageType.ShouldEqual(ProgressMessageTypes.Failed);
            _mockHub.ProgressWithMessages.Count.ShouldEqual(2);
            _mockHub.ProgressWithMessages[0].OptionalMessage.MessageText.ShouldEqual("The running action has a system level problem.");
        }

        [Test]
        public async void Check11ThrowExceptionHalfWayThroughOk()
        {

            //SETUP
            var data = new CommsTestActionData
            {
                Mode = TestServiceModes.ThrowExceptionHalfWayThrough,
                NumIterations = 2,
                SecondsBetweenIterations = 0,
            };
            var hr = new HubRunner<int,CommsTestActionData>("aaa", typeof(ICommsTestActionNormal), data, false);

            //ATTEMPT
            var finalMess = await hr.RunActionAsync("aaa", "123", _mockHub);

            //VERIFY
            finalMess.MessageText.ShouldEqual("Failed with 1 error");
            finalMess.MessageType.ShouldEqual(ProgressMessageTypes.Failed);
            _mockHub.ProgressWithMessages.Count.ShouldEqual(4);
            _mockHub.ProgressWithMessages[2].OptionalMessage.MessageText.ShouldEqual("The running action has a system level problem.");
        }

        [Test]
        public async void Check15CancelAtStartOk()
        {

            //SETUP
            var data = new CommsTestActionData
            {
                Mode = TestServiceModes.RunSuccessfully,
                NumIterations = 2,
                SecondsBetweenIterations = 0,
            };
            var hr = new HubRunner<int,CommsTestActionData>("aaa", typeof(ICommsTestActionNormal), data, false);
            
            //ATTEMPT
            hr.CancellationPending = true;
            var finalMess = await hr.RunActionAsync("aaa", "123", _mockHub);

            //VERIFY
            finalMess.MessageText.ShouldEqual("Cancelled by user.");
            finalMess.MessageType.ShouldEqual(ProgressMessageTypes.Cancelled);
            _mockHub.ProgressWithMessages.Count.ShouldEqual(2);
            _mockHub.ProgressWithMessages.Last().OptionalMessage.MessageType.ShouldEqual(ProgressMessageTypes.Cancelled);
        }

        [Test]
        public async void Check20DisposeWasCalledOk()
        {

            //SETUP
            //SETUP
            var data = new CommsTestActionData
            {
                Mode = TestServiceModes.RunSuccessfully,
                NumIterations = 2,
                SecondsBetweenIterations = 0,
            };
            var hr = new HubRunner<int,CommsTestActionData>("aaa", typeof(ICommsTestActionNormal), data, false);

            //ATTEMPT
            await hr.RunActionAsync("aaa", "123", _mockHub);

            //VERIFY
            _action.DisposeWasCalled.ShouldEqual(true);
        }

        //-------------------------------------
        //check sumbit called

        [Test]
        public async void Test30RunEmptyTestActionNoSubmitOk()
        {
            //SETUP
            var data = new Tag
            {
                TagId = 0
            };
            var hr = new HubRunner<int, Tag>("aaa", typeof(IEmptyTestAction), data, false);

            //ATTEMPT
            var lastMessage = await hr.RunActionAsync("aaa", "123", _mockHub);

            //VERIFY
            lastMessage.MessageType.ShouldEqual(ProgressMessageTypes.Finished);
            lastMessage.MessageText.ShouldEqual("Successful");
            _dummyDb.SaveChangesWithValidationCalled.ShouldEqual(false);
        }

        [Test]
        public async void Test31RunEmptyTestActionWithSubmitOk()
        {
            //SETUP
            var data = new Tag
            {
                TagId = 0
            };
            var hr = new HubRunner<int, Tag>("aaa", typeof(IEmptyTestActionWithSubmit), data, false);

            //ATTEMPT
            var lastMessage = await hr.RunActionAsync("aaa", "123", _mockHub);

            //VERIFY
            lastMessage.MessageType.ShouldEqual(ProgressMessageTypes.Finished);
            lastMessage.MessageText.ShouldEqual("Successful... and written to database.");
            _dummyDb.SaveChangesWithValidationCalled.ShouldEqual(true);
        }
    }
}
