#region licence
// The MIT License (MIT)
// 
// Filename: SelectListOfUsers.cs
// Date Created: 2014/08/22
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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.Owin;

namespace SampleWebApp.Identity
{
    public class SelectListOfUsers
    {

        /// <summary>
        /// This contains the strings to show and the value to return if that string is selected
        /// </summary>
        public List<KeyValuePair<string, string>> KeyValueList { get; private set; }

        /// <summary>
        /// This is the value returned from the list
        /// </summary>
        [Required]
        public string SelectedValue { get; set; }

        public SelectListOfUsers(HttpContextBase httpContext)
        {
            var currUserName = httpContext.User == null ? null : httpContext.User.Identity.Name;
            var userManager = httpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            KeyValueList =
                userManager.Users.ToList().Select(x => new KeyValuePair<string, string>(x.DisplayName, x.Email)).ToList();
            KeyValueList.Insert(0, new KeyValuePair<string, string>("-- not logging in --", string.Empty));

            SelectedValue = KeyValueList.Any(x => x.Value == currUserName)
                ? KeyValueList.First(x => x.Value == currUserName).Value
                : KeyValueList.First().Value;
        }
    }
}