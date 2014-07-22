using System.Linq;
using Autofac;
using BizLayer.BBCScheduleService;
using BizLayer.BBCScheduleService.Concrete;
using BizLayer.BlogsAnalysis;
using BizLayer.BlogsAnalysis.Concrete;
using BizLayer.Startup;
using DataLayer.DataClasses;
using DataLayer.DataClasses.Concrete;
using DataLayer.Startup;
using GenericServices;
using GenericServices.Services;
using GenericServices.Services.Concrete;
using NUnit.Framework;
using SampleWebApp.Infrastructure;
using ServiceLayer.PostServices.Concrete;
using ServiceLayer.Startup;
using Tests.Helpers;

namespace Tests.UnitTests.Group03ServiceLayer
{
    [TestFixture]
    public class Test11AutoFacModules
    {

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            using (var db = new SampleWebAppDb())
            {
                DataLayerInitialise.InitialiseThis();
                DataLayerInitialise.ResetDatabaseToTestData(db, TestDataSelection.Small);
            }
        }


        //-------------------------------------
        //DataLayer

        [Test]
        public void CheckSetupDbContextLifetimeScopeItems()
        {
            //SETUP
            var builder = new ContainerBuilder();
            builder.RegisterModule( new DataLayerModule());
            var container = builder.Build();

            //ATTEMPT & VERIFY
            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var instance1 = lifetimeScope.Resolve<IDbContextWithValidation>();
                var instance2 = lifetimeScope.Resolve<IDbContextWithValidation>();
                Assert.NotNull(instance1);
                (instance1 is SampleWebAppDb).ShouldEqual(true);
                Assert.AreSame(instance1, instance2);                       //check that lifetimescope is working
            }
        }

        [Test]
        public void CheckSetupDbContextLifetimeScopeViaSampleWebAppDb()
        {
            //SETUP
            var builder = new ContainerBuilder();
            builder.RegisterModule(new DataLayerModule());
            var container = builder.Build();

            //ATTEMPT & VERIFY
            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var instance1 = lifetimeScope.Resolve<IDbContextWithValidation>();
                var instance2 = lifetimeScope.Resolve<SampleWebAppDb>();
                Assert.NotNull(instance1);
                (instance1 is SampleWebAppDb).ShouldEqual(true);
                Assert.AreSame(instance1, instance2);                       //check the two different items are the same
            }
        }

        [Test]
        public void CheckSetupBizLayerScopeItems()
        {
            //SETUP
            var builder = new ContainerBuilder();
            builder.RegisterModule(new DataLayerModule());
            builder.RegisterModule(new BizLayerModule());
            var container = builder.Build();

            //ATTEMPT & VERIFY
            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var example1 = lifetimeScope.Resolve<IBlogAnalyser>();
                var example2 = lifetimeScope.Resolve<IBlogAnalyser>();
                Assert.NotNull(example1);
                Assert.AreNotSame(example1, example2);                       //check transient
                (example1 is BlogAnalyser).ShouldEqual(true);
            }

        }

        //---------------------------------------------
        //ServiceLayer, which also resolves DataLayer

        [Test]
        public void Test10ServiceSetupServiceLayer()
        {
            //SETUP
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ServiceLayerModule());
            var container = builder.Build();

            //ATTEMPT & VERIFY
            CheckExampleServicesResolve(container);
        }


        [Test]
        public void Test15SetupServiceLayerDirectGenerics()
        {
            //SETUP
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ServiceLayerModule());
            var container = builder.Build();

            //ATTEMPT & VERIFY
            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var instance = lifetimeScope.Resolve<IListService>();
                Assert.NotNull(instance);
                (instance is ListService).ShouldEqual(true);
            }
        }

        [Test]
        public void Test16UseServiceLayerDirectGenerics()
        {
            //SETUP
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ServiceLayerModule());
            var container = builder.Build();

            //ATTEMPT & VERIFY
            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var service = lifetimeScope.Resolve<IListService>();
                var posts = service.GetList<Post>().ToList();
                posts.Count.ShouldEqual(3);
            }
        }

        //------------------------------------------------------
        //MVC layer

        [Test]
        public void Test20ViaMvcSetup()
        {
            //SETUP
            var container = AutofacDi.SetupDependency();

            //ATTEMPT & VERIFY
            CheckExampleServicesResolve(container);

        }

        //-------------------------------------------------------
        //private helper

        private static void CheckExampleServicesResolve(IContainer container)
        {
            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                //DataLayer - Data classes
                //var dataClass1 = lifetimeScope.Resolve<ISmService>();
                //var dataClass2 = lifetimeScope.Resolve<ISmService>();
                //Assert.NotNull(dataClass1);
                //Assert.AreNotSame(dataClass1, dataClass2);                       //check transient
                //(dataClass1 is SmService).ShouldEqual(true);

                //DataLayer - repositories
                var db1 = lifetimeScope.Resolve<IDbContextWithValidation>();
                var db2 = lifetimeScope.Resolve<IDbContextWithValidation>();
                Assert.NotNull(db1);
                Assert.AreSame(db1, db2);                       //check that lifetimescope is working

                ////BizLayer
                //var bizclass1 = lifetimeScope.Resolve<IDecodeBuildTestScheme>();
                //var bizclass2 = lifetimeScope.Resolve<IDecodeBuildTestScheme>();
                //Assert.NotNull(bizclass1);
                //Assert.AreNotSame(bizclass1, bizclass2);                       //check transient
                //(bizclass1 is DecodeBuildTestScheme).ShouldEqual(true);

                //ServiceLayer - simple
                var example1 = lifetimeScope.Resolve<IScheduleSearcherAsync>();
                var example2 = lifetimeScope.Resolve<IScheduleSearcherAsync>();
                Assert.NotNull(example1);
                Assert.AreNotSame(example1, example2);                       //check transient
                (example1 is ScheduleSearcherAsync).ShouldEqual(true);

                //ServiceLayer - complex
                var service1 = lifetimeScope.Resolve<IListService>();
                var service2 = lifetimeScope.Resolve<IListService>();
                Assert.NotNull(service1);
                Assert.AreNotSame(service1, service2);                       //check transient
                (service1 is ListService).ShouldEqual(true);
            }
        }
    }
}
