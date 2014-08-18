using System.ComponentModel.DataAnnotations;

namespace SampleWebApp.Models
{
    public class AttendeeAllListModel
    {
        [UIHint("HiddenInput")]
        public int AttendeeId { get; set; }

        public string CourseName { get; set; }

        public string FullName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        //This column has been specifically included so we can add a column based security constraint
        public bool HasPaid { get; set; }


    }
}