#region licence
// The MIT License (MIT)
// 
// Filename: Test11AutoFacModules.cs
// Date Created: 2014/05/22
// 
// Copyright (c) 2014 Jon Smith (www.selectiveanalytics.com & www.thereformedprogrammer.net)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion
using System.Linq;
using Autofac;
using BizLayer.BBCScheduleService;
using BizLayer.BBCScheduleService.Concrete;
using BizLayer.BlogsAnalysis;
using BizLayer.BlogsAnalysis.Concrete;
using BizLayer.Startup;
using DataLayer.BBCScheduleService;
using DataLayer.BBCScheduleService.Concrete;
using DataLayer.DataClasses;
using DataLayer.DataClasses.Concrete;
using DataLayer.Startup;
using GenericServices;
using GenericServices.Services.Concrete;
using NUnit.Framework;
using SampleWebApp.Infrastructure;
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
                DataLayerInitialise.InitialiseThis(false, true);
                DataLayerInitialise.ResetBlogs(db, TestDataSelection.Small);
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
                var instance1 = lifetimeScope.Resolve<IGenericServicesDbContext>();
                var instance2 = lifetimeScope.Resolve<IGenericServicesDbContext>();
                Assert.NotNull(instance1);
                (instance1 is SampleWebAppDb).ShouldEqual(true);
                Assert.AreSame(instance1, instance2);                       //check that lifetimescope is working
            }
        }

        [Test]
        public void CheckSetupSecureSampleWebAppDb()
        {
            //SETUP
            var builder = new ContainerBuilder();
            builder.RegisterModule(new DataLayerModule());
            var container = builder.Build();

            //ATTEMPT & VERIFY
            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var instance1 = lifetimeScope.Resolve<IGenericServicesDbContext>();
                Assert.NotNull(instance1);
                (instance1 is SecureSampleWebAppDb).ShouldEqual(true);
            }
        }

        [Test]
        public void CheckSetupDifferentSampleWebAppDb()
        {
            //SETUP
            var builder = new ContainerBuilder();
            builder.RegisterModule(new DataLayerModule());
            var container = builder.Build();

            //ATTEMPT & VERIFY
            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var instance1 = lifetimeScope.Resolve<IGenericServicesDbContext>();
                var instance2 = lifetimeScope.Resolve<SampleWebAppDb>();
                Assert.NotNull(instance1);
                (instance1 is SecureSampleWebAppDb).ShouldEqual(true);
                (instance2 is SampleWebAppDb).ShouldEqual(true);
                Assert.AreNotEqual(instance1, instance2);                       //check the two different items are different
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
                var posts = service.GetAll<Post>().ToList();
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
                var dataClass1 = lifetimeScope.Resolve<IRadio4Fm>();
                var dataClass2 = lifetimeScope.Resolve<IRadio4Fm>();
                Assert.NotNull(dataClass1);
                Assert.AreNotSame(dataClass1, dataClass2);                       //check transient
                (dataClass1 is Radio4Fm).ShouldEqual(true);

                //DataLayer - repositories
                var db1 = lifetimeScope.Resolve<IGenericServicesDbContext>();
                var db2 = lifetimeScope.Resolve<IGenericServicesDbContext>();
                Assert.NotNull(db1);
                Assert.AreSame(db1, db2);                       //check that lifetimescope is working

                ////BizLayer
                var bizclass1 = lifetimeScope.Resolve<IBlogAnalyser>();
                var bizclass2 = lifetimeScope.Resolve<IBlogAnalyser>();
                Assert.NotNull(bizclass1);
                Assert.AreNotSame(bizclass1, bizclass2);                       //check transient
                (bizclass1 is BlogAnalyser).ShouldEqual(true);

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
