using System.Collections.ObjectModel;

namespace BizLayer.BBCScheduleService.Concrete
{
    public class ScheduleSearchResult
    {

        public ScheduleSearcherData SearchCriteria { get; private set; }

        public Collection<DayOfProgrammes> FoundDaysCollection { get; private set; }

        public ScheduleSearchResult(ScheduleSearcherData searchCriteria, Collection<DayOfProgrammes> foundDaysCollection)
        {
            SearchCriteria = searchCriteria;
            FoundDaysCollection = foundDaysCollection;
        }
    }
}
