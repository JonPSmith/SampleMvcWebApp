using BizLayer.BBCScheduleService.Concrete;
using GenericServices.ActionComms;

namespace BizLayer.BBCScheduleService
{
    public interface IScheduleSearcherAsync : IActionCommsAsync<ScheduleSearchResult, ScheduleSearcherData>
    {
        bool SubmitChangesOnSuccess { get; }
        ActionFlags ActionConfig { get; }
        int LowerBound { get; set; }
        int UpperBound { get; set; }
    }
}