#region licence
// The MIT License (MIT)
// 
// Filename: Test10RunnerSetup.cs
// Date Created: 2014/06/26
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
            var data = new SimpleTagDto
            {
                Name = "Must Exist",
                Slug = "MustExistNoSpaces"
            };
            var rs = new RunnerSetup<SimpleTagDto>(typeof(IEmptyTestAction), data);

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
            //SETUP
            var data = new SimpleTagDtoAsync
            {
                Name = "Must Exist",
                Slug = "MustExistNoSpaces"
            };
            var rs = new RunnerSetup<SimpleTagDtoAsync>(typeof(IEmptyTestActionAsync), data);

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
            var rs = new RunnerSetup<SimpleTagDto>(typeof(IEmptyTestAction), new SimpleTagDto(InstrumentedOpFlags.FailOnUpdateDataFromDto));

            //ATTEMPT
            var json = rs.SetupAndReturnJsonResult(new DummyIDbContextWithValidation());

            //VERIFY
            var js = json.Data.SerialiseToJson();
            js.ShouldEqual("{\"errorsDict\":{\"\":{\"errors\":[\"Flag was set to fail in UpdateDataFromDto.\"]}}}");
        }


        [Test]
        public void Test26RunnerAsyncDecodeDtoOk()
        {
            //SETUP
            var rs = new RunnerSetup<SimpleTagDtoAsync>(typeof(IEmptyTestActionAsync), new SimpleTagDtoAsync(InstrumentedOpFlags.FailOnUpdateDataFromDto));

            //ATTEMPT
            var json = rs.SetupAndReturnJsonResult(new DummyIDbContextWithValidation());

            //VERIFY
            var js = json.Data.SerialiseToJson();
            js.ShouldEqual("{\"errorsDict\":{\"\":{\"errors\":[\"Flag was set to fail in UpdateDataFromDto.\"]}}}");
        }
    }
}
