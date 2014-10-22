#region licence
// The MIT License (MIT)
// 
// Filename: Test10DiSimple.cs
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
using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using NUnit.Framework;
using Tests.DependencyItems;
using Tests.Helpers;

namespace Tests.UnitTests.Group03ServiceLayer
{
    class Test10DiSimple
    {

        [Test]
        public void Test01AutoFacSimple()
        {
            //SETUP
            var builder = new ContainerBuilder();
            builder.RegisterType<SimpleClass>().As<ISimpleClass>();
            var container = builder.Build();

            //ATTEMPT & VERIFY
            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var instance = lifetimeScope.Resolve<ISimpleClass>();
                Assert.NotNull(instance);
                (instance is SimpleClass).ShouldEqual(true);
            }

        }

        [Test]
        public void Test02AutoFacTransient()
        {
            //Setup
            var builder = new ContainerBuilder();
            builder.RegisterType<SimpleClass>().As<ISimpleClass>();
            var container = builder.Build();

            //Attempt
            ISimpleClass instance1;
            using (var lifetimeScope = container.BeginLifetimeScope())
                instance1 = lifetimeScope.Resolve<ISimpleClass>();
            ISimpleClass instance2;
            using (var lifetimeScope = container.BeginLifetimeScope())
                instance2 = lifetimeScope.Resolve<ISimpleClass>();

            //Verify
            Assert.NotNull(instance1);
            Assert.NotNull(instance2);
            Assert.AreNotSame(instance1, instance2);

        }


        [Test]
        public void Test03AutoFacSingle()
        {
            //Setup
            var builder = new ContainerBuilder();
            builder.RegisterType<SimpleClass>().As<ISimpleClass>().SingleInstance();
            var container = builder.Build();

            //Attempt
            ISimpleClass instance1;
            using (var lifetimeScope = container.BeginLifetimeScope())
                instance1 = lifetimeScope.Resolve<ISimpleClass>();
            ISimpleClass instance2;
            using (var lifetimeScope = container.BeginLifetimeScope())
                instance2 = lifetimeScope.Resolve<ISimpleClass>();

            //Verify
            Assert.NotNull(instance1);
            Assert.NotNull(instance2);
            Assert.AreSame(instance1, instance2);

        }

        [Test]
        public void Test04AutoFacLifeTimeScope()
        {
            //Setup
            var builder = new ContainerBuilder();
            builder.RegisterType<SimpleClass>().As<ISimpleClass>().InstancePerLifetimeScope();
            var container = builder.Build();

            //Attempt and VERIFY
            ISimpleClass scope1Instance1;
            ISimpleClass scope1Instance2;
            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                scope1Instance1 = lifetimeScope.Resolve<ISimpleClass>();
                scope1Instance2 = lifetimeScope.Resolve<ISimpleClass>();
                Assert.NotNull(scope1Instance1);
                Assert.NotNull(scope1Instance2);
                Assert.AreSame(scope1Instance1, scope1Instance2);
            }

            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                ISimpleClass scope2Instance1 = lifetimeScope.Resolve<ISimpleClass>();
                Assert.NotNull(scope2Instance1);
                Assert.NotNull(scope1Instance1);
                Assert.NotNull(scope1Instance2);
                Assert.AreNotSame(scope1Instance1, scope2Instance1);
                Assert.AreNotSame(scope1Instance1, scope2Instance1);
            }

        }

        //-----------------------------------------------------------
        //item with constructor param

        [Test]
        public void Test05AutoFacConstructor()
        {
            //SETUP
            var builder = new ContainerBuilder();
            builder.RegisterType<ConstructorParamClass>().As<IConstructorParamClass>()
                .WithParameter("myInt", 42);
            var container = builder.Build();

            //ATTEMPT & VERIFY
            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var instance = lifetimeScope.Resolve<IConstructorParamClass>();
                Assert.NotNull(instance);
                instance.MyInt.ShouldEqual(42);
            }

        }


        //-----------------------------------------------------------
        //tests on IDisposable items

        private int _numTimeDisposeCalled;

        [Test]
        public void Test15AutoFacDisposeCreate()
        {
            //Setup
            var builder = new ContainerBuilder();
            Action checker = (() => _numTimeDisposeCalled++);
            builder.RegisterType<MyDisposableClass>().As<IMyDisposableClass>().WithParameter("disposeWasCalled", checker);
            var container = builder.Build();

            //Attempt
            _numTimeDisposeCalled = 0;
            var mydisp = container.Resolve<IMyDisposableClass>();

            //Verify
            Assert.NotNull(mydisp);
            _numTimeDisposeCalled.ShouldEqual(0);

        }

        [Test]
        public void Test16AutoFacDisposeCalled()
        {
            //Setup
            var builder = new ContainerBuilder();
            Action checker = (() => _numTimeDisposeCalled++);
            builder.RegisterType<MyDisposableClass>().As<IMyDisposableClass>().WithParameter("disposeWasCalled", checker);
            var container = builder.Build();

            //Attempt
            _numTimeDisposeCalled = 0;
            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var mydisp = lifetimeScope.Resolve<IMyDisposableClass>();
                Assert.NotNull(mydisp);
            }

            //Verify
            _numTimeDisposeCalled.ShouldEqual(1);

        }

        //--------------------------------------------------------------
        //register generic 

        [Test]
        public void Test20AutoFacRegisterGeneric()
        {
            //SETUP
            var builder = new ContainerBuilder();
            builder.RegisterGeneric(typeof(GenericInterface<>)).As(typeof(IGenericInterface<>));
            var container = builder.Build();

            //ATTEMPT & VERIFY
            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var instance = lifetimeScope.Resolve<IGenericInterface<SimpleClass>>();
                Assert.NotNull(instance);
                (instance is GenericInterface<SimpleClass>).ShouldEqual(true);
                instance.GetTypeName().ShouldEqual(typeof(SimpleClass).Name);
            }

        }

        [Test]
        public void Test21AutoFacRegisterGenericAfterRegisterAssembly()
        {
            //SETUP
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(GetType().Assembly).AsImplementedInterfaces();
            builder.RegisterGeneric(typeof(GenericInterface<>)).As(typeof(IGenericInterface<>));
            var container = builder.Build();

            //ATTEMPT & VERIFY
            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var instance = lifetimeScope.Resolve<IGenericInterface<SimpleClass>>();
                Assert.NotNull(instance);
                (instance is GenericInterface<SimpleClass>).ShouldEqual(true);
                instance.GetTypeName().ShouldEqual(typeof(SimpleClass).Name);
            }

        }

        //---------------------------------------------------------
        //tests on what happens if ctor is private

        [Test]
        public void Test30AutoFacRegisterClassWithPrivateCtorBad()
        {
            //SETUP
            var builder = new ContainerBuilder();
            builder.RegisterType<ClassWithPrivateCtor>().As<IClassWithPrivateCtor>();
            var container = builder.Build();

            //ATTEMPT & VERIFY
            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var ex = Assert.Throws<Autofac.Core.DependencyResolutionException>(() => lifetimeScope.Resolve<IClassWithPrivateCtor>());
                ex.Message.ShouldStartWith("No constructors on type");
            }

        }

        [Test]
        public void Test31AutoFacTryCtorWithPrivateCtorAsOptionOk()
        {
            //SETUP
            var builder = new ContainerBuilder();
            builder.RegisterType<ClassWithPrivateCtor>().As<IClassWithPrivateCtor>();
            builder.RegisterGeneric(typeof (ClassToTestClassWithPrivateCtor<>))
                .As(typeof (IClassToTestClassWithPrivateCtor<>))
                .OnActivating(e =>
                {
                    var interfaceToLookup = e.Instance.GetType().GetGenericArguments()[0];
                    var resolvedInterface =
                        e.Context.ComponentRegistry.RegistrationsFor(new TypedService(interfaceToLookup)).SingleOrDefault();
                    ((ISetType)e.Instance).SetType(resolvedInterface.Activator.LimitType);
                });
            var container = builder.Build();

            //ATTEMPT & VERIFY
            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var instance = lifetimeScope.Resolve<IClassToTestClassWithPrivateCtor<IClassWithPrivateCtor>>();
                Assert.NotNull(instance);
                (instance is ClassToTestClassWithPrivateCtor<IClassWithPrivateCtor>).ShouldEqual(true);
            }

        }
    }
}
