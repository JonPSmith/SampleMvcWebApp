using System;
using System.Threading.Tasks;
using DataLayer.BBCScheduleService.Concrete;

namespace DataLayer.BBCScheduleService
{
    public interface IRadio4Fm
    {
        Radio4Fm.ResponseTypes ResponseType { get; set; }
        
        Task<string> GetSchedule(DateTime day);
    }
}