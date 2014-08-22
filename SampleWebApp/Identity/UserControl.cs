using System;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace SampleWebApp.Identity
{
    public static class UserControl
    {

        public static void ChangeUser(this HttpContextBase httpContext, string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                httpContext.GetOwinContext().Authentication.SignOut();
                return;
            }

            var userManager = httpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = userManager.FindByEmail(email);
            if (user == null)
                throw new InvalidOperationException(string.Format("Failed to find user with email {0}", email));

            var signInManager = httpContext.GetOwinContext().Get<ApplicationSignInManager>();

            var result = signInManager.PasswordSignIn(email, user.OriginalPassword, false, shouldLockout: false);
            if (result != SignInStatus.Success)
                throw new InvalidOperationException(string.Format("Failed to login user with email {0}", email));

        }
    }
}