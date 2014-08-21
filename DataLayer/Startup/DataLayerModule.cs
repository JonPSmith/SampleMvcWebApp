using System.Runtime.CompilerServices;
using Autofac;
using DataLayer.DataClasses;
using GenericServices;

[assembly: InternalsVisibleTo("Tests")]

namespace DataLayer.Startup
{
    public class DataLayerModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {

            //Autowire the classes with interfaces
            builder.RegisterAssemblyTypes(GetType().Assembly).AsImplementedInterfaces();

            //set Entity Framework context to instance per lifetime scope. 
            //This is important as we get one context per lifetime, so all db classes are tracked together.
            builder.RegisterType<SecureSampleWebAppDb>().As<IDbContextWithValidation>().InstancePerLifetimeScope();
            builder.RegisterType<SampleWebAppDb>().As<SampleWebAppDb>().InstancePerLifetimeScope();
        }
    }
}
