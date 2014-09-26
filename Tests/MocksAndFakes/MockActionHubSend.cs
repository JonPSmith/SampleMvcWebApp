#region licence
// The MIT License (MIT)
// 
// Filename: MockActionHubSend.cs
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
using System.Collections.Generic;
using System.Linq;
using GenericServices.ActionComms;
using SampleWebApp.ActionProgress;

namespace Tests.MocksAndFakes
{
    public class MockActionHubSend : IActionHubSend
    {

        private readonly List<ProgressWithOptionalMessage> _logOfCalls = new List<ProgressWithOptionalMessage>();

        public string ActionConfigFlags { get; private set; }

        public IReadOnlyList<ProgressWithOptionalMessage> AllProgressAndMessages { get {  return _logOfCalls.AsReadOnly();} }

        public IReadOnlyList<ProgressWithOptionalMessage> ProgressWithMessages { get { return _logOfCalls.Where(x => x.OptionalMessage != null).ToList().AsReadOnly(); } }

        public IEnumerable<int> PercentagesOnly { get { return _logOfCalls.Select(x => x.PercentageDone); } }

        public object FinalJsonData { get; private set; }

        public void Started(IHubControl actionRunner, string actionConfig)
        {
            ActionConfigFlags = actionConfig;
            _logOfCalls.Add(new ProgressWithOptionalMessage(-1, null));
        }

        public void Progress(IHubControl actionRunner, int percentageDone, ProgressMessage message)
        {
            _logOfCalls.Add(new ProgressWithOptionalMessage(percentageDone, message));
        }

        public void Stopped(IHubControl actionRunner, ProgressMessage finalMessage, object jsonToSend)
        {
            _logOfCalls.Add(new ProgressWithOptionalMessage(101, finalMessage));
            FinalJsonData = jsonToSend;
        }
    }
}
