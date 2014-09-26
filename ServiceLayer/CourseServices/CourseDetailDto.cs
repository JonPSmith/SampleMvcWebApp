#region licence
// The MIT License (MIT)
// 
// Filename: CourseDetailDto.cs
// Date Created: 2014/08/16
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
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataLayer.DataClasses.Concrete;
using GenericSecurity;
using GenericServices;
using GenericServices.Core;
using GenericServices;

namespace ServiceLayer.CourseServices
{
    public class CourseDetailDto : EfGenericDto<Course, CourseDetailDto>
    {

        [UIHint("HiddenInput")]
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

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        public int LengthDays { get; set; }

        [DisplayName("Attendees")]
        public string AttendeesNames { get; private set; }

        protected override ServiceFunctions SupportedFunctions
        {
            get { return ServiceFunctions.AllCrudButList | ServiceFunctions.DoesNotNeedSetup; }
        }

        /// <summary>
        /// We need to override this because we must only read the attendess name and not select the other
        /// properties in attendees as they have sql security controls on their columns
        /// </summary>
        /// <param name="context"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        protected override ISuccessOrErrors<CourseDetailDto> CreateDtoAndCopyDataIn(IGenericServicesDbContext context, Expression<Func<Course, bool>> predicate)
        {
            Mapper.CreateMap<Course, CourseDetailDto>();
            var status = GetDataUntracked(context).Where(predicate).Project().To<CourseDetailDto>().RealiseSingleWithErrorChecking();
            if (!status.IsValid)
                return status;

            //now we just read the name beacause the other columns are protected 
            //(need to use TryManyWithPermissionChecking as not logged in cannot see anything)
            var localStatus =
                context.Set<Attendee>().Where(x => x.CourseId == status.Result.CourseId).Select(x => x.FullName).RealiseManyWithErrorChecking();
            if (!localStatus.IsValid)
                return status.SetErrors(localStatus.Errors.Select( x => x.ErrorMessage));

            status.Result.AttendeesNames = string.Join(", ", localStatus.Result);

            return status;
        }
    }
}
