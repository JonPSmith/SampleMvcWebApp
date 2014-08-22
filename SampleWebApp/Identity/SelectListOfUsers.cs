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