using System.ComponentModel.DataAnnotations;
using DataLayer.DataClasses.Concrete;
using GenericServices.Core;

namespace ServiceLayer.AttendeeServices
{
    public class AttendeeUpdateNotPaidDto : EfGenericDto<Attendee, AttendeeUpdateNotPaidDto>
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

        //In this dto HasPaid is never be updated
        public bool HasPaid { get; internal set; }

        protected override ServiceFunctions SupportedFunctions
        {
            //we do not support create because create means a booking
            get { return ServiceFunctions.Update | ServiceFunctions .Detail | ServiceFunctions.DoesNotNeedSetup; }
        }
    }
}
