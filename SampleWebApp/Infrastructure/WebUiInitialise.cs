using System.Web;
using System.Web.Mvc;
using Autofac.Integration.Mvc;
using GenericServices.Logger;
using SampleWebApp.ActionProgress;
using ServiceLayer.Startup;

namespace SampleWebApp.Infrastructure
{
    public static class WebUiInitialise
    {

        /// <summary>
        /// This should be called at Startup
        /// </summary>
        public static void InitialiseThis(HttpApplication application)
        {

            //WE set up the log4net settings from a local file and then assign the logger to GenericServices.GenericLoggerFactory
            var log4NetPath = application.Server.MapPath("~/Log4Net.xml");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(log4NetPath));

            GenericServices.GenericLoggerFactory.SetLoggerMethod = name => new Log4NetGenericLogger(name);

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

    }
}