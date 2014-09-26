#region licence
// The MIT License (MIT)
// 
// Filename: Test05HubRunnerBasic.cs
// Date Created: 2014/05/31
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
using DataLayer.DataClasses.Concrete;
using GenericServices;
using GenericServices.ActionComms;
using log4net.Appender;
using log4net.Config;
using NUnit.Framework;
using SampleWebApp.ActionProgress;
using SampleWebApp.Infrastructure;
using Tests.Helpers;
using Tests.MocksAndFakes;

namespace Tests.UnitTests.Group08ActionRunner
{
    class Test05HubRunnerBasic
    {

        private MemoryAppender _log4NetMemoryLog;

        [TestFixtureSetUp]
        public void SetupFixture()
        {
            _log4NetMemoryLog = new MemoryAppender();
            BasicConfigurator.Configure(_log4NetMemoryLog);
            ServicesConfiguration.SetLoggerMethod = name => new Log4NetGenericLogger(name);
        }

        [SetUp]
        public void Setup()
        {
            ActionHub.LifeTimeScopeProvider = () => new MockHubDependencyResolver();
            _log4NetMemoryLog.Clear();
        }

        [Test]
        public void Test01CheckHubRunnerCtorOk()
        {
            //SETUP

            //ATTEMPT
            var hr = new HubRunner<int,Tag>("aaa", typeof(IEmptyTestAction), new Tag(), false);

            //VERIFY
            hr.ShouldNotEqualNull(); 
        }

        [Test]
        public void Test02CheckHubRunnerCtorBad()
        {
            //SETUP
            ActionHub.LifeTimeScopeProvider = null;

            //ATTEMPT
            var ex = Assert.Throws<NullReferenceException>( () => new HubRunner<int,Tag>("aaa", typeof(IEmptyTestAction), new Tag(), false));

            //VERIFY
            ex.Message.ShouldStartWith("You must set up the static varable HubRunner.LifeTimeScopeProvider");
        }

        //----------------------------------------------------------

        [Test]
        public async void Test05CheckRunActionOk()
        {
            //SETUP
            var hr = new HubRunner<int,Tag>("aaa", typeof(IEmptyTestAction), new Tag(), false);
            var mockHub = new MockActionHubSend();

            //ATTEMPT
            var lastMessage = await hr.RunActionAsync("aaa", "123", mockHub);

            //VERIFY
            lastMessage.MessageType.ShouldEqual(ProgressMessageTypes.Finished);
            var logs = _log4NetMemoryLog.GetEvents();
            Assert.GreaterOrEqual(logs.Length, 2);
            logs[0].Level.ShouldEqual( log4net.Core.Level.Info );
            logs[1].Level.ShouldEqual(log4net.Core.Level.Info);
        }

        [Test]
        public async void Test06CheckRunActionFail()
        {
            //SETUP
            var hr = new HubRunner<int,Tag>("aaa", typeof(IEmptyTestAction), new Tag{ TagId = 2}, false);      //TagId = 2 means force fail
            var mockHub = new MockActionHubSend();

            //ATTEMPT
            var lastMessage = await hr.RunActionAsync("aaa", "123", mockHub);

            //VERIFY
            lastMessage.MessageType.ShouldEqual(ProgressMessageTypes.Failed);
            var logs = _log4NetMemoryLog.GetEvents();
            Assert.GreaterOrEqual(logs.Length, 2);
            logs[0].Level.ShouldEqual(log4net.Core.Level.Info);
            logs[1].Level.ShouldEqual(log4net.Core.Level.Warn);
        }

    }
}
