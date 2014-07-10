using GenericServices;
using GenericServices.ActionComms;
using ServiceLayer.BBCScheduleService.Concrete;

namespace ServiceLayer.BBCScheduleService
{
    public interface IScheduleSearcherAsync : IActionCommsAsync<ScheduleSearchResult, ScheduleSearcherData>
    {
        bool SubmitChangesOnSuccess { get; }
        ActionFlags ActionConfig { get; }
        int LowerBound { get; set; }
        int UpperBound { get; set; }
    }
}