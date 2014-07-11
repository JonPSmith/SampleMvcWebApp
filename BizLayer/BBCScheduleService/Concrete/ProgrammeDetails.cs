using System;
using System.Linq;
using System.Xml.Linq;

namespace BizLayer.BBCScheduleService.Concrete
{
    public class ProgrammeDetails
    {
        /// <summary>
        /// Name of the programme
        /// </summary>
        public string Title { get; private set; }

        public string Synopsis { get; private set; }

        public TimeSpan StartTime { get; private set; }

        public TimeSpan Duration { get; private set; }

        public ProgrammeDetails(XElement xmlBroadcast)
        {
            var programme = xmlBroadcast.Element("programme");
            Synopsis = programme.Element("short_synopsis").Value;
            var displayTitles = programme.Element("display_titles");
            Title = displayTitles.Elements("title").First().Value;
            StartTime = DateTime.Parse(xmlBroadcast.Element("start").Value).TimeOfDay;
            var endTime = DateTime.Parse(xmlBroadcast.Element("end").Value).TimeOfDay;
            Duration = endTime - StartTime;
        }
    }
}
