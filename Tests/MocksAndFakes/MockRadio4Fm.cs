using System;
using System.Threading.Tasks;
using DataLayer.BBCScheduleService;
using Tests.Helpers;

namespace Tests.MocksAndFakes
{
    public class MockRadio4Fm : IRadio4Fm
    {
        public Radio4Fm.ResponseTypes ResponseType { get; set; }
        public async Task<string> GetSchedule(DateTime day)
        {
            return TestFileHelpers.GetTestFileContent("TestRadio4 schedule 2014-7-3.xml"); 
        }
    }
}
