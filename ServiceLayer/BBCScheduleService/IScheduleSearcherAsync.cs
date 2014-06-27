using GenericServices;
using ServiceLayer.BBCScheduleService.Concrete;

namespace ServiceLayer.BBCScheduleService
{
    public interface IScheduleSearcherAsync : IActionAsync<ScheduleSearchResult, ScheduleSearcherData>
    {
        bool SubmitChangesOnSuccess { get; }
        ActionFlags ActionConfig { get; }
        int LowerBound { get; set; }
        int UpperBound { get; set; }
    }
}