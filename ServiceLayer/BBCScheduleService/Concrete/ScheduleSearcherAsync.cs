using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DataLayer.BBCScheduleService;
using GenericServices;
using GenericServices.Actions;
using GenericServices.Core;
using GenericServices.ServicesAsync;

namespace ServiceLayer.BBCScheduleService.Concrete
{
    public class ScheduleSearcherAsync : ActionBase, IScheduleSearcherAsync
    {
        private IRadio4Fm _reader;
        private string _searchString;

        public override bool SubmitChangesOnSuccess { get { return false; } }

        public override ActionFlags ActionConfig { get { return ActionFlags.ExitOnSuccess; } }

        public ScheduleSearcherAsync(IRadio4Fm reader)
        {
            _reader = reader;
        }

        public async Task<ISuccessOrErrors<ScheduleSearchResult>> DoActionAsync(IActionComms actionComms, ScheduleSearcherData searchCriteria)
        {
            ISuccessOrErrors<ScheduleSearchResult> status = new SuccessOrErrors<ScheduleSearchResult>();

            ReportProgressAndCheckCancel(actionComms, 0,
                new ProgressMessage(ProgressMessageTypes.Info, searchCriteria.ToString()));

            var dateToSearch = searchCriteria.Today;
            var daysFound = new Collection<DayOfProgrammes>();
            var searchFunc = SetupSearchFunc(searchCriteria);
            for (int i = 0; i <= searchCriteria.NumDaysAhead; i++)
            {
                //get data
                var xmlString = await _reader.GetSchedule(dateToSearch);
                daysFound.Add( new DayOfProgrammes(xmlString, searchFunc));

                ReportProgressAndCheckCancel(actionComms, i / (searchCriteria.NumDaysAhead+1),
                    new ProgressMessage(ProgressMessageTypes.Info, "Searched schedule on {0}", dateToSearch.ToShortDateString()));
                dateToSearch = dateToSearch.AddDays(1);
            }
            var searchResult = new ScheduleSearchResult(searchCriteria, daysFound);
            return status.SetSuccessWithResult(searchResult, "Found {0} programmes over {1} day{2}",
                daysFound.SelectMany( x => x.Programmes).Count(),
                searchCriteria.NumDaysAhead + 1,
                searchCriteria.NumDaysAhead == 0 ? "" : "s");
        }

        private Func<string,bool> SetupSearchFunc(ScheduleSearcherData searchCriteria)
        {
            if (searchCriteria.AcceptAllProgrammes)
                return AcceptAll;

            _searchString = searchCriteria.TitleWordSearch;
            return FilterByTitle;
        }

        private bool AcceptAll(string title)
        {
            return true;
        }

        private bool FilterByTitle(string title)
        {
            return title.Contains(_searchString);
        }
    }
}
