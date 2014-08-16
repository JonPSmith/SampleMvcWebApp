using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.DataClasses.Concrete
{
    public class Course
    {

        public int CourseId { get; set; }

        [MaxLength(255)]
        [Required]
        public string Name { get; set; }

        [MaxLength(255)]
        [Required]
        public string MainPresenter { get; set; }

        [MaxLength(2048)]
        [Required]
        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public int LengthDays { get; set; }

        //---------------------------------------
        //relationships

        public ICollection<Attendee> Attendees { get; set; }

        public override string ToString()
        {
            return string.Format("Name: {0}, StartDate: {1}, LengthDays: {2}", Name, StartDate, LengthDays);
        }
    }
}
