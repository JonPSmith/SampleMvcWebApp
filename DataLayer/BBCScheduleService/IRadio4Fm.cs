using System;
using System.Threading.Tasks;

namespace DataLayer.BBCScheduleService
{
    public interface IRadio4Fm
    {
        Radio4Fm.ResponseTypes ResponseType { get; set; }
        
        Task<string> GetSchedule(DateTime day);
    }
}