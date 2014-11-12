#region licence
// The MIT License (MIT)
// 
// Filename: WebUiInitialise.cs
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
using System;
using System.Web;
using System.Web.Mvc;
using Autofac.Integration.Mvc;
using GenericLibsBase;
using SampleWebApp.Properties;
using ServiceLayer.Startup;

namespace SampleWebApp.Infrastructure
{
    public enum HostTypes { NotSet, LocalHost, WebWiz, Azure };

    public static class WebUiInitialise
    {
        private const bool ResetIndentityDatabase = false;          //set this to true to reset content of Identity database

        //Note: This is also used when running locally
        private const string WebWizLog4NetRelPath = "~/Log4Net.xml";

        public const string DatabaseConnectionStringName = "SampleWebAppDb";

        /// <summary>
        /// This provides the host we 
        /// </summary>
        public static HostTypes HostType { get; private set; }

        /// <summary>
        /// This should be called at Startup
        /// </summary>
        public static void InitialiseThis(HttpApplication application)
        {
            HostType = DecodeHostType(Settings.Default.HostTypeString);
            //WebWiz does not allow drop/create database
            var canDropCreateDatabase = HostType != HostTypes.WebWiz;

            SetupLogging(application, HostType);

            //This runs the ServiceLayer initialise, whoes job it is to initialise any of the lower layers
            //NOTE: This MUST to come before the setup of the DI because it relies on the configInfo being set up
            ServiceLayerInitialise.InitialiseThis(HostType == HostTypes.Azure, canDropCreateDatabase); 

            //This sets up the Autofac container for all levels in the program
            var container = AutofacDi.SetupDependency();

            //// Set the dependency resolver for MVC.
            var mvcResolver = new AutofacDependencyResolver(container);
            DependencyResolver.SetResolver(mvcResolver);
        }

        private static HostTypes DecodeHostType(string hostTypeString)
        {
            HostTypes hostType ;
            Enum.TryParse(hostTypeString, true, out hostType);
            return hostType;
        }

        private static void SetupLogging(HttpApplication application, HostTypes hostType)
        {

            switch (hostType)
            {
                case HostTypes.NotSet:
                    //we do not set up the logging
                    break;
                case HostTypes.LocalHost:
                    //LocalHost uses WebWiz setup for log4Net
                case HostTypes.WebWiz:
                    //We set up the log4net settings from a local file and then assign the logger to GenericServices.GenericLoggerFactory
                    var log4NetPath = application.Server.MapPath(WebWizLog4NetRelPath);
                    log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(log4NetPath));
                    GenericLibsBaseConfig.SetLoggerMethod = name => new Log4NetGenericLogger(name);
                    break;
                case HostTypes.Azure:
                    //we use the TraceGenericLogger when in Azure
                    GenericLibsBaseConfig.SetLoggerMethod = name => new TraceGenericLogger(name);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("hostType");
            }

            GenericLibsBaseConfig.GetLogger("LoggerSetup").Info("We have just assegned a logger.");
        }
    }
}