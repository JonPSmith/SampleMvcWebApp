using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.BBCScheduleService.Concrete
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
