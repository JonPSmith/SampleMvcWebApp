#region licence
// The MIT License (MIT)
// 
// Filename: Test02ScheduleSearcher.cs
// Date Created: 2014/07/11
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
using BizLayer.BBCScheduleService.Concrete;
using NUnit.Framework;
using Tests.Helpers;
using Tests.MocksAndFakes;

namespace Tests.UnitTests.Group02BizLayer
{
    class Test02ScheduleSearcher
    {

        [Test]
        public async void Test01ScheduleSearcherAll()
        {
            //SETUP
            var ss = new ScheduleSearcherAsync(new MockRadio4Fm());

            //ATTEMPT
            var data = new ScheduleSearcherData();
            var status = await ss.DoActionAsync(null, data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.SuccessMessage.ShouldEqual("Found 52 programmes over 1 day");
        }

        [Test]
        public async void Test02ScheduleSearcherFilter()
        {
            //SETUP
            var ss = new ScheduleSearcherAsync(new MockRadio4Fm());

            //ATTEMPT
            var data = new ScheduleSearcherData
            {
                TitleWordSearch = "News"
            };
            var status = await ss.DoActionAsync(null, data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.SuccessMessage.ShouldEqual("Found 5 programmes over 1 day");
        }

        [Test]
        public async void Test05CheckCollection()
        {
            //SETUP
            var ss = new ScheduleSearcherAsync(new MockRadio4Fm());

            //ATTEMPT
            var data = new ScheduleSearcherData();
            var status = await ss.DoActionAsync(null, data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.Result.FoundDaysCollection.Count.ShouldEqual(1);
        }

        [Test]
        public async void Test06CheckCollectionDate()
        {
            //SETUP
            var ss = new ScheduleSearcherAsync(new MockRadio4Fm());

            //ATTEMPT
            var data = new ScheduleSearcherData();
            var status = await ss.DoActionAsync(null, data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.Result.FoundDaysCollection.Count.ShouldEqual(1);
            status.Result.FoundDaysCollection[0].Day.ShouldEqual( new DateTime(2014,7,3));
        }

        [Test]
        public async void Test07CheckCollectionFilter()
        {
            //SETUP
            var ss = new ScheduleSearcherAsync(new MockRadio4Fm());

            //ATTEMPT
            var data = new ScheduleSearcherData
            {
                TitleWordSearch = "News"
            };
            var status = await ss.DoActionAsync(null, data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.Result.FoundDaysCollection.Count.ShouldEqual(1);
            status.Result.FoundDaysCollection[0].Programmes.All(x => x.Title.Contains(data.TitleWordSearch));
        }

        //----------------------------------------------

        [Test]
        public async void Test10CheckMultiDays()
        {
            //SETUP
            var ss = new ScheduleSearcherAsync(new MockRadio4Fm());

            //ATTEMPT
            var data = new ScheduleSearcherData
            {
                NumDaysAhead = 1
            };
            var status = await ss.DoActionAsync(null, data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.Result.FoundDaysCollection.Count.ShouldEqual(2);
        }

    }
}
