using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace ServiceLayer.BBCScheduleService.Concrete
{
    public class DayOfProgrammes
    {

        public DateTime Day { get; private set; }

        public Collection<ProgrammeDetails> Programmes { get; private set; }

        public DayOfProgrammes(string xmlString, Func<string,bool> FilterByTitle )
        {
            var xmlDay = XElement.Parse(xmlString).Element("day");
            Day = DateTime.Parse(xmlDay.Attribute("date").Value).Date;
            Programmes = new Collection<ProgrammeDetails>(
                xmlDay.Element("broadcasts").Elements("broadcast").Select(x => new ProgrammeDetails(x)).ToList()
                .Where(x => FilterByTitle(x.Title)).ToList());
        }

    }
}
