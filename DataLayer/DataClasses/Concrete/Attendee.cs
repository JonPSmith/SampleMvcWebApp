using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.DataClasses.Concrete
{
    public class Attendee
    {

        public int AttendeeId { get; set; }

        [Required]
        [MaxLength(128)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(128)]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        //This column has been specifically included so we can add a column based security constraint
        public bool HasPaid { get; set; }

        //---------------------------------------
        //relationships

        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course BookedOn { get; set; }


        //ctors
        public Attendee() {}

        public Attendee(string fullName, string emailAddress, bool hasPaid, Course bookedOn)
        {
            FullName = fullName;
            EmailAddress = emailAddress;
            HasPaid = hasPaid;
            BookedOn = bookedOn;
        }

        public override string ToString()
        {
            return string.Format("FullName: {0}, HasPaid: {1}, BookedOn: {2}", 
                FullName, HasPaid, BookedOn == null ? "-- unknown --" : BookedOn.Name);
        }
    }
}
