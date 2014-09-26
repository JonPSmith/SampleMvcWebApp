#region licence
// The MIT License (MIT)
// 
// Filename: ScheduleSearcherAsync.cs
// Date Created: 2014/07/11
// 
// Copyright (c) 2014 Jon Smith (www.selectiveanalytics.com & www.thereformedprogrammer.net)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.BBCScheduleService;
using GenericServices;
using GenericServices.ActionComms;
using GenericServices.Core;

namespace BizLayer.BBCScheduleService.Concrete
{
    public class ScheduleSearcherAsync : ActionCommsBase, IScheduleSearcherAsync
    {
        private readonly IRadio4Fm _reader;
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
            //case insensitive search
            return title.IndexOf(_searchString, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
