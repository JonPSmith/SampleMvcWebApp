using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SampleWebApp.Identity;
using SampleWebApp.Infrastructure;

namespace SampleWebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //This allows interfaces etc to be provided as parameters to action methods
            ModelBinders.Binders.DefaultBinder = new DiModelBinder();

            //Now call the method to initialise anything that is required before startup (which includes setting up DI)
            WebUiInitialise.InitialiseThis(this);

            //Set the parameter to true if you want the Identity database completely reset, otherwise false 
            Database.SetInitializer(new IdentityDbInitializer(false));

        }
    }
}
