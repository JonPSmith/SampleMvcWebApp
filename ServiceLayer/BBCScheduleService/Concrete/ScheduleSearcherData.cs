using System;
using System.ComponentModel.DataAnnotations;

namespace ServiceLayer.BBCScheduleService.Concrete
{
    public class ScheduleSearcherData 
    {

        internal DateTime Today { get { return DateTime.Today; } }

        [Range(0, 7, ErrorMessage = "The search can only look from today forward for a week")]
        public int NumDaysAhead { get; set; }

        /// <summary>
        /// A word or phrase to find in the programme title
        /// </summary>
        public string TitleWordSearch { get; set; }

        public bool AcceptAllProgrammes 
        {
            get { return string.IsNullOrEmpty(TitleWordSearch); }
        }

        public override string ToString()
        {
            var startString = NumDaysAhead == 0
                ? "Looking at today's BBC Radio4 schedule"
                : string.Format("Looking at BBC Radio4 schedule for next {0} days ", NumDaysAhead);

            return AcceptAllProgrammes
                ? startString + " listing"
                : string.Format("{0} for any programmes with '{1}' in its title", startString, TitleWordSearch);
        }
    }
}
