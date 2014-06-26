using System.Collections;
using Autofac;
using DataLayer.DataClasses.Concrete;
using GenericServices;
using GenericServices.Actions;
using GenericServices.Logger;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using NUnit.Framework;
using SampleWebApp.ActionProgress;
using ServiceLayer.TestActionService;
using ServiceLayer.TestActionService.Concrete;
using Tests.DependencyItems;
using Tests.Helpers;
using Tests.MocksAndFakes;

namespace Tests.UnitTests.Group08ActionRunner
{
    class Test06HubRunnerDi
    {
        private MemoryAppender _log4NetMemoryLog;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<SimpleClass>().As<ISimpleClass>();
            builder.RegisterType<EmptyTestAction>().As<IEmptyTestAction>();
            builder.RegisterType<EmptyTestActionAsync>().As<IEmptyTestActionAsync>();
            builder.RegisterType<CommsTestActionNormal>().As<ICommsTestActionNormal>();
            builder.RegisterType<CommsTestActionNoCancelEtc>().As<ICommsTestActionNoCancelEtc>();
            var container = builder.Build();
            ActionHub.LifeTimeScopeProvider = () => new AutoFacActionHubResolver(container);

            _log4NetMemoryLog = new MemoryAppender();
            BasicConfigurator.Configure(_log4NetMemoryLog);
            GenericLoggerFactory.SetLoggerMethod = name => new Log4NetGenericLogger(name);
        }

        [SetUp]
        public void Setup()
        {
            _log4NetMemoryLog.Clear();
        }

        [Test]
        public async void Test01CheckDiEmptyTestActionOk()
        {
            //SETUP
            var hr = new HubRunner<int,Tag>("aaa", typeof(IEmptyTestAction), new Tag(), false);
            var mockHub = new MockActionHubSend();

            //ATTEMPT
            var lastMessage = await hr.RunActionAsync("aaa", "123", mockHub);

            //VERIFY
            lastMessage.MessageType.ShouldEqual(ProgressMessageTypes.Finished);
            lastMessage.MessageText.ShouldEqual("Successful");
        }

        [Test]
        public async void Test02CheckDiNonRegisteredInterfaceOk()
        {
            //SETUP
            var hr = new HubRunner<int, Tag>("aaa", typeof(IEnumerable), new Tag(), false);
            var mockHub = new MockActionHubSend();

            //ATTEMPT
            var lastMessage = await hr.RunActionAsync("aaa", "123", mockHub);

            //VERIFY
            lastMessage.MessageType.ShouldEqual(ProgressMessageTypes.Failed);
            lastMessage.MessageText.ShouldEqual("Failed with 1 error");
            var logs = _log4NetMemoryLog.GetEvents();
            logs[0].Level.ShouldEqual(Level.Fatal);
            logs[0].RenderedMessage.ShouldEqual("Action of interface type 'IEnumerable' had an exception.");
        }

        [Test]
        public async void Test03CheckDiIncorrectInterfaceOk()
        {
            //SETUP
            var hr = new HubRunner<int, Tag>("aaa", typeof(ISimpleClass), new Tag(), false);
            var mockHub = new MockActionHubSend();

            //ATTEMPT
            var lastMessage = await hr.RunActionAsync("aaa", "123", mockHub);

            //VERIFY
            lastMessage.MessageType.ShouldEqual(ProgressMessageTypes.Failed);
            lastMessage.MessageText.ShouldEqual("Failed with 1 error");
            var logs = _log4NetMemoryLog.GetEvents();
            logs[0].Level.ShouldEqual(Level.Fatal);
            logs[0].RenderedMessage.ShouldEqual("The interface 'ISimpleClass' did not resolve to IActionDefn<Int32,Tag>");
        }

        [Test]
        public async void Test04CheckEmptyTestActionConfigOk()
        {
            //SETUP
            var hr = new HubRunner<int,Tag>("aaa", typeof(IEmptyTestAction), new Tag(), false);
            var mockHub = new MockActionHubSend();

            //ATTEMPT
            var lastMessage = await hr.RunActionAsync("aaa", "123", mockHub);

            //VERIFY
            mockHub.ActionConfigFlags.ShouldEqual("NoProgressSent, NoMessagesSent, CancelNotSupported");
        }

        [Test]
        public async void Test05CheckDiEmptyTestActionAsyncOk()
        {
            //SETUP
            var hr = new HubRunner<int, Tag>("aaa", typeof(IEmptyTestActionAsync), new Tag(), true);
            var mockHub = new MockActionHubSend();

            //ATTEMPT
            var lastMessage = await hr.RunActionAsync("aaa", "123", mockHub);

            //VERIFY
            lastMessage.MessageType.ShouldEqual(ProgressMessageTypes.Finished);
            lastMessage.MessageText.ShouldEqual("Successful");
        }


        //--------------------------

        [Test]
        public async void Test10CheckDiCommsTestActionOk()
        {
            //SETUP
            var data = new CommsTestActionData
            {
                SecondsBetweenIterations = 0
            };
            var hr = new HubRunner<int,CommsTestActionData>("aaa", typeof(ICommsTestActionNormal), data, false);
            var mockHub = new MockActionHubSend();

            //ATTEMPT
            var lastMessage = await hr.RunActionAsync("aaa", "123", mockHub);

            //VERIFY
            lastMessage.MessageType.ShouldEqual(ProgressMessageTypes.Finished);
        }

        [Test]
        public async void Test11CheckCommsTestActionConfigNormalOk()
        {
            //SETUP
            var data = new CommsTestActionData
            {
                SecondsBetweenIterations = 0
            };
            var hr = new HubRunner<int,CommsTestActionData>("aaa", typeof(ICommsTestActionNormal), data, false);
            var mockHub = new MockActionHubSend();

            //ATTEMPT
            var lastMessage = await hr.RunActionAsync("aaa", "123", mockHub);

            //VERIFY
            mockHub.ActionConfigFlags.ShouldEqual("Normal");
        }

        [Test]
        public async void Test12CheckCommsTestActionConfigNoMessagesOk()
        {
            //SETUP
            var data = new CommsTestActionData
            {
                SecondsBetweenIterations = 0
            };
            var hr = new HubRunner<int,CommsTestActionData>("aaa", typeof(ICommsTestActionNormal), data, false);
            var mockHub = new MockActionHubSend();

            //ATTEMPT
            var lastMessage = await hr.RunActionAsync("aaa", "123", mockHub);

            //VERIFY
            mockHub.ActionConfigFlags.ShouldEqual("Normal");
        }

        //--------------------------------------

        [Test]
        public async void Test15CheckDiCommsTestActionNoCancelEtcOk()
        {
            //SETUP
            var data = new CommsTestActionData
            {
                SecondsBetweenIterations = 0
            };
            var hr = new HubRunner<int,CommsTestActionData>("aaa", typeof(ICommsTestActionNoCancelEtc), data, false);
            var mockHub = new MockActionHubSend();

            //ATTEMPT
            var lastMessage = await hr.RunActionAsync("aaa", "123", mockHub);

            //VERIFY
            lastMessage.MessageType.ShouldEqual(ProgressMessageTypes.Finished);
        }

        [Test]
        public async void Test16CheckCommsTestActionNoCancelEtcConfigNoMessagesOk()
        {
            //SETUP
            var data = new CommsTestActionData
            {
                SecondsBetweenIterations = 0
            };
            var hr = new HubRunner<int,CommsTestActionData>("aaa", typeof(ICommsTestActionNoCancelEtc), data, false);
            var mockHub = new MockActionHubSend();

            //ATTEMPT
            var lastMessage = await hr.RunActionAsync("aaa", "123", mockHub);

            //VERIFY
            mockHub.ActionConfigFlags.ShouldEqual("ExitOnSuccess, NoProgressSent, NoMessagesSent, CancelNotSupported");
        }

    }
}
