using System;
using System.Linq;
using NUnit.Framework;
using ServiceLayer.BBCScheduleService.Concrete;
using Tests.Helpers;
using Tests.MocksAndFakes;

namespace Tests.UnitTests.Group03ServiceLayer
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
