using System.ComponentModel.DataAnnotations;
using DataLayer.DataClasses.Concrete;
using GenericServices.Core;

namespace ServiceLayer.AttendeeServices
{
    public class AttendeeNotPaidDto : EfGenericDto<Attendee, AttendeeNotPaidDto>
    {
        [UIHint("HiddenInput")]
        public int AttendeeId { get; set; }

        [Required]
        [MaxLength(128)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(128)]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        protected override ServiceFunctions SupportedFunctions
        {
            //we do not support create because create means a booking
            get { return ServiceFunctions.Update | ServiceFunctions .Detail | ServiceFunctions.DoesNotNeedSetup; }
        }
    }
}
