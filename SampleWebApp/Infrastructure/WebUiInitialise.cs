using System;
using System.Web;
using System.Web.Mvc;
using Autofac.Integration.Mvc;
using SampleWebApp.ActionProgress;
using SampleWebApp.Identity;
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
            ServiceLayerInitialise.InitialiseThis(false, canDropCreateDatabase); 

            //Set the first parameter to true to setup/reset the Identity database, otherwise false to use what is there
            InitialiseIdentityDb.Initialise(ResetIndentityDatabase, canDropCreateDatabase);

            //This sets up the Autofac container for all levels in the program
            var container = AutofacDi.SetupDependency();

            //// Set the dependency resolver for MVC.
            var mvcResolver = new AutofacDependencyResolver(container);
            DependencyResolver.SetResolver(mvcResolver);

            //Now set the Action dependency resolver
            ActionHub.LifeTimeScopeProvider = () => new AutoFacActionHubResolver(container);

        }

        /// <summary>
        /// This returns the connection string of the database
        /// </summary>
        /// <returns></returns>
        public static string GetDbConnectionString()
        {
            return
                System.Configuration.ConfigurationManager.ConnectionStrings[WebUiInitialise.DatabaseConnectionStringName
                    ].ConnectionString;
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
                    GenericServices.ServicesConfiguration.SetLoggerMethod = name => new Log4NetGenericLogger(name);
                    break;
                case HostTypes.Azure:
                    //we use the TraceGenericLogger when in Azure
                    GenericServices.ServicesConfiguration.SetLoggerMethod = name => new TraceGenericLogger(name);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("hostType");
            }
        }
    }
}