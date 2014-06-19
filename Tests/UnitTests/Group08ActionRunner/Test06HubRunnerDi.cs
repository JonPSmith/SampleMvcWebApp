using Autofac;
using DataLayer.DataClasses.Concrete;
using GenericServices.Actions;
using NUnit.Framework;
using SampleWebApp.ActionProgress;
using ServiceLayer.TestActionService;
using ServiceLayer.TestActionService.Concrete;
using Tests.Helpers;
using Tests.MocksAndFakes;

namespace Tests.UnitTests.Group08ActionRunner
{
    class Test06HubRunnerDi
    {
        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<EmptyTestAction>().As<IEmptyTestAction>();
            builder.RegisterType<CommsTestAction>().As<ICommsTestAction>();
            var container = builder.Build();
            ActionHub.LifeTimeScopeProvider = () => new AutoFacActionHubResolver(container);
        }

        [Test]
        public void Test01CheckDiEmptyTestActionOk()
        {
            //SETUP
            var hr = new HubRunner<Tag>("aaa", typeof(IEmptyTestAction), new Tag());
            var mockHub = new MockActionHubSend();

            //ATTEMPT
            var lastMessage = hr.RunActionSynchronously("aaa", "123", mockHub);

            //VERIFY
            lastMessage.MessageType.ShouldEqual(ProgressMessageTypes.Finished);
        }

        [Test]
        public void Test02CheckEmptyTestActionConfigOk()
        {
            //SETUP
            var hr = new HubRunner<Tag>("aaa", typeof(IEmptyTestAction), new Tag());
            var mockHub = new MockActionHubSend();

            //ATTEMPT
            var lastMessage = hr.RunActionSynchronously("aaa", "123", mockHub);

            //VERIFY
            mockHub.ActionConfigFlags.ShouldEqual("NoProgressSent, NoMessagesSent, CancelNotSupported");
        }

        [Test]
        public void Test10CheckDiCommsTestActionOk()
        {
            //SETUP
            var data = new CommsTestActionData
            {
                SecondsBetweenIterations = 0
            };
            var hr = new HubRunner<CommsTestActionData>("aaa", typeof(ICommsTestAction), data);
            var mockHub = new MockActionHubSend();

            //ATTEMPT
            var lastMessage = hr.RunActionSynchronously("aaa", "123", mockHub);

            //VERIFY
            lastMessage.MessageType.ShouldEqual(ProgressMessageTypes.Finished);
        }

        [Test]
        public void Test11CheckCommsTestActionConfigNormalOk()
        {
            //SETUP
            var data = new CommsTestActionData
            {
                SecondsBetweenIterations = 0
            };
            var hr = new HubRunner<CommsTestActionData>("aaa", typeof(ICommsTestAction), data);
            var mockHub = new MockActionHubSend();

            //ATTEMPT
            var lastMessage = hr.RunActionSynchronously("aaa", "123", mockHub);

            //VERIFY
            mockHub.ActionConfigFlags.ShouldEqual("Normal");
        }

        [Test]
        public void Test12CheckCommsTestActionConfigNoMessagesOk()
        {
            //SETUP
            var data = new CommsTestActionData
            {
                SecondsBetweenIterations = 0
            };
            var hr = new HubRunner<CommsTestActionData>("aaa", typeof(ICommsTestAction), data);
            var mockHub = new MockActionHubSend();

            //ATTEMPT
            var lastMessage = hr.RunActionSynchronously("aaa", "123", mockHub);

            //VERIFY
            mockHub.ActionConfigFlags.ShouldEqual("Normal");
        }

    }
}
