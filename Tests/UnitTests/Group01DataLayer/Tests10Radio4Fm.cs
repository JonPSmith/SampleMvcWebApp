using System;
using System.Xml.Linq;
using DataLayer.BBCScheduleService;
using DataLayer.BBCScheduleService.Concrete;
using NUnit.Framework;

namespace Tests.UnitTests.Group01DataLayer
{
    class Tests10Radio4Fm
    {

        [Test]
        public async void Test01ScheduleSearcherAll()
        {
            //SETUP
            var r = new Radio4Fm();

            //ATTEMPT
            var data = await r.GetSchedule(DateTime.Today);

            //VERIFY
            var xml = XElement.Parse(data);
        }

    }
}
