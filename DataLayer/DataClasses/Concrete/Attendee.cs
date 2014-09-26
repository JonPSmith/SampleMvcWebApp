#region licence
// The MIT License (MIT)
// 
// Filename: Attendee.cs
// Date Created: 2014/08/14
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
