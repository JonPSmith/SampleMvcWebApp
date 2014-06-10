using System.Runtime.CompilerServices;
using Autofac;
using ServiceLayer.Startup;

[assembly: InternalsVisibleTo("Tests")]

namespace SampleWebApp.Infrastructure
{
    internal static class AutofacDi
    {

        private static IContainer _container;

        internal static IContainer SetupDependency()
        {

            var builder = new ContainerBuilder();
            Load(builder);

            _container = builder.Build();

            return _container;
        }

        private static void Load(ContainerBuilder builder)
        {
            //register the service layer, which then registers all other dependencies in the rest of the system
            builder.RegisterModule(new ServiceLayerModule());
        }
    }
}