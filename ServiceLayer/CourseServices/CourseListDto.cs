using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataLayer.DataClasses.Concrete;
using GenericServices.Core;

namespace ServiceLayer.CourseServices
{
    public class CourseListDto : EfGenericDto<Course, CourseListDto>
    {
        [UIHint("HiddenInput")]
        public int CourseId { get; set; }

        [MaxLength(255)]
        [Required]
        public string Name { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        public int LengthDays { get; set; }

        [DisplayName("#Attendees (provisional)")]
        public int AttendeesCount { get; set; }

        protected override ServiceFunctions SupportedFunctions
        {
            get { return ServiceFunctions.List | ServiceFunctions.DoesNotNeedSetup; }
        }
    }
}
