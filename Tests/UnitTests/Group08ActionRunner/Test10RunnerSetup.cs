using System;
using System.Collections.Generic;
using DataLayer.DataClasses.Concrete;
using GenericServices.Core;
using NUnit.Framework;
using SampleWebApp.ActionProgress;
using Tests.DependencyItems;
using Tests.Helpers;
using Tests.MocksAndFakes;

namespace Tests.UnitTests.Group08ActionRunner
{
    class Test10RunnerSetup
    {

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            ActionHub.LifeTimeScopeProvider = () => new MockHubDependencyResolver();
        }


        [Test]
        public void Test01RunnerSetupCtorOk()
        {
            //SETUP

            //ATTEMPT
            var rs = new RunnerSetup<Tag>(typeof (IEmptyTestAction), new Tag());

            //VERIFY
        }

        [Test]
        public void Test02RunnerSetupAsyncCtorOk()
        {
            //SETUP

            //ATTEMPT
            var rs = new RunnerSetup<Tag>(typeof(IEmptyTestActionAsync), new Tag());

            //VERIFY
        }

        [Test]
        public void Test05RunnerSetupNotInterfaceBad()
        {
            //SETUP

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => new RunnerSetup<Tag>(typeof(int), new Tag()));

            //VERIFY
            ex.Message.ShouldEqual("The TActionI must be an interface to work properly.");
        }


        [Test]
        public void Test06RunnerSetupNotGenericBad()
        {
            //SETUP

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => new RunnerSetup<Tag>(typeof(ISimpleClass), new Tag()));

            //VERIFY
            ex.Message.ShouldEqual("The interface must have IActionDefn<TOut,Tin> or IActionDefnAsync<TOut,Tin> as the first sub-interface.");
        }

        [Test]
        public void Test06RunnerSetupWrongInterfaceBad()
        {
            //SETUP

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => new RunnerSetup<Tag>(typeof(IList<int>), new Tag()));

            //VERIFY
            ex.Message.ShouldEqual("The interface must have IActionDefn<TOut,Tin> or IActionDefnAsync<TOut,Tin> as the first sub-interface.");
        }

        //--------------------------------------------------------------------
        //Now test where data is the task data, i.e. no dto copy

        [Test]
        public void Test10RunnerDecodeOk()
        {
            //SETUP
            var rs = new RunnerSetup<Tag>(typeof(IEmptyTestAction), new Tag());

            //ATTEMPT
            var json = rs.SetupAndReturnJsonResult(null);

            //VERIFY
            var js = json.Data.SerialiseToJson();
            js.ShouldStartWith("{\"ActionGuid\":");
        }

        [Test]
        public void Test11RunnerAsyncDecodeOk()
        {
            //SETUP
            var rs = new RunnerSetup<Tag>(typeof(IEmptyTestActionAsync), new Tag());

            //ATTEMPT
            var json = rs.SetupAndReturnJsonResult(null);

            //VERIFY
            var js = json.Data.SerialiseToJson();
            js.ShouldStartWith("{\"ActionGuid\":");
        }

        //--------------------------------------------------------------------
        //Now test where data is a dto

        [Test]
        public void Test20RunnerDecodeDtoOk()
        {
            //SETUP
            var rs = new RunnerSetup<SimpleTagDto>(typeof(IEmptyTestAction), new SimpleTagDto());

            //ATTEMPT
            var json = rs.SetupAndReturnJsonResult(new DummyIDbContextWithValidation());

            //VERIFY
            var js = json.Data.SerialiseToJson();
            js.ShouldStartWith("{\"ActionGuid\":");
        }

        [Test]
        public void Test21RunnerAsyncDecodeDtoOk()
        {
            //SETUP
            var rs = new RunnerSetup<SimpleTagDtoAsync>(typeof(IEmptyTestActionAsync), new SimpleTagDtoAsync());

            //ATTEMPT
            var json = rs.SetupAndReturnJsonResult(new DummyIDbContextWithValidation());

            //VERIFY
            var js = json.Data.SerialiseToJson();
            js.ShouldStartWith("{\"ActionGuid\":");
        }


        //------------------

        [Test]
        public void Test25RunnerDecodeDtoFail()
        {
            //SETUP
            var rs = new RunnerSetup<SimpleTagDto>(typeof(IEmptyTestAction), new SimpleTagDto(InstrumentedOpFlags.FailOnCopyDtoToData));

            //ATTEMPT
            var json = rs.SetupAndReturnJsonResult(new DummyIDbContextWithValidation());

            //VERIFY
            var js = json.Data.SerialiseToJson();
            js.ShouldEqual("{\"errorsDict\":{\"\":{\"errors\":[\"Flag was set to fail in CopyDtoToData.\"]}}}");
        }


        [Test]
        public void Test26RunnerAsyncDecodeDtoOk()
        {
            //SETUP
            var rs = new RunnerSetup<SimpleTagDtoAsync>(typeof(IEmptyTestActionAsync), new SimpleTagDtoAsync(InstrumentedOpFlags.FailOnCopyDtoToData));

            //ATTEMPT
            var json = rs.SetupAndReturnJsonResult(new DummyIDbContextWithValidation());

            //VERIFY
            var js = json.Data.SerialiseToJson();
            js.ShouldEqual("{\"errorsDict\":{\"\":{\"errors\":[\"Flag was set to fail in CopyDtoToDataAsync.\"]}}}");
        }
    }
}
