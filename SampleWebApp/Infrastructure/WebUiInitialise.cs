using System;
using System.Web;
using System.Web.Mvc;
using Autofac.Integration.Mvc;
using SampleWebApp.ActionProgress;
using SampleWebApp.Properties;
using ServiceLayer.Startup;

namespace SampleWebApp.Infrastructure
{
    public enum HostTypes { NotSet, LocalHost, WebWiz, Azure };

    public static class WebUiInitialise
    {

        //Note: This is also used when running locally
        private const string WebWizLog4NetRelPath = "~/Log4Net.xml";

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

            SetupLogging(application, HostType);

            //This runs the ServiceLayer initialise, whoes job it is to initialise any of the lower layers
            //NOTE: This MUST to come before the setup of the DI because it relies on the configInfo being set up
            ServiceLayerInitialise.InitialiseThis();

            //This sets up the Autofac container for all levels in the program
            var container = AutofacDi.SetupDependency();

            //// Set the dependency resolver for MVC.
            var mvcResolver = new AutofacDependencyResolver(container);
            DependencyResolver.SetResolver(mvcResolver);

            //Now set the Action dependency resolver
            ActionHub.LifeTimeScopeProvider = () => new AutoFacActionHubResolver(container);

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
                    GenericServices.GenericLoggerFactory.SetLoggerMethod = name => new Log4NetGenericLogger(name);
                    break;
                case HostTypes.Azure:
                    //we use the TraceGenericLogger when in Azure
                    GenericServices.GenericLoggerFactory.SetLoggerMethod = name => new TraceGenericLogger(name);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("hostType");
            }
        }
    }
}