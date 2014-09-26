#region licence
// The MIT License (MIT)
// 
// Filename: UserControl.cs
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