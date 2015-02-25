#region licence
// The MIT License (MIT)
// 
// Filename: BundleConfig.cs
// Date Created: 2014/05/20
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

using System.Web.Optimization;

namespace SampleWebApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/javascript").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      //"~/Content/notify.css", - not used any more
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            //------------------------------------------------------------------
            //the bundles below were once used but not any more. Left as I might need them for the code that has been moved out to another library

            //Combined with bootstrap.js into one javascript bundle
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));
            //
            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //    "~/Scripts/bootstrap.js",
            //    "~/Scripts/respond.js"));

            //Not used anymore
            //bundles.Add(new ScriptBundle("~/bundles/ActionRunner").Include(
            //    "~/Scripts/jquery-notify.js",
            //    "~/Scripts/jquery-ui-{version}.js",
            //    "~/Scripts/jquery.signalR-{version}.js",
            //    "~/Scripts/ActionRunnerComms.js",
            //    "~/Scripts/ActionRunnerUi.js"));

            //Not used
            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //Not used any more
            ////Commented out the parts that are not used from the jQuery UI
            //bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
            //  "~/Content/themes/base/jquery.ui.core.css",
            //  "~/Content/themes/base/jquery.ui.resizable.css",
            //  "~/Content/themes/base/jquery.ui.selectable.css",
            //  //"~/Content/themes/base/jquery.ui.accordion.css",
            //  //"~/Content/themes/base/jquery.ui.autocomplete.css",
            //  //"~/Content/themes/base/jquery.ui.button.css",
            //  "~/Content/themes/base/jquery.ui.dialog.css",
            //  //"~/Content/themes/base/jquery.ui.slider.css",
            //  //"~/Content/themes/base/jquery.ui.tabs.css",
            //  //"~/Content/themes/base/jquery.ui.datepicker.css",
            //  "~/Content/themes/base/jquery.ui.progressbar.css",
            //  "~/Content/themes/base/jquery.ui.theme.css"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            //BundleTable.EnableOptimizations = true;
        }
    }
}
