using Autofac;
using DataLayer.Startup;
using GenericServices;
using GenericServices.Services;
using GenericServices.ServicesAsync;

namespace ServiceLayer.Startup
{
    public class ServiceLayerModule : Module
    {

        /// <summary>
        /// This registers all items in service layer and below
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {

            //Now register the DataLayer
            builder.RegisterModule(new DataLayerModule());

            //Reigister the BizLayer
            //builder.RegisterModule(new BizLayerModule());

            //---------------------------
            //Register service layer: autowire all 
            builder.RegisterAssemblyTypes(GetType().Assembly).AsImplementedInterfaces();

            //and register the open generics
            builder.RegisterGeneric(typeof(CreateService<>)).As(typeof(ICreateService<>));
            builder.RegisterGeneric(typeof(DeleteService<>)).As(typeof(IDeleteService<>));
            builder.RegisterGeneric(typeof(DetailService<>)).As(typeof(IDetailService<>));
            builder.RegisterGeneric(typeof(ListService<>)).As(typeof(IListService<>));
            builder.RegisterGeneric(typeof(UpdateService<>)).As(typeof(IUpdateService<>));

            builder.RegisterGeneric(typeof(CreateServiceAsync<>)).As(typeof(ICreateServiceAsync<>));
            builder.RegisterGeneric(typeof(DeleteServiceAsync<>)).As(typeof(IDeleteServiceAsync<>));
            builder.RegisterGeneric(typeof(DetailServiceAsync<>)).As(typeof(IDetailServiceAsync<>));
            builder.RegisterGeneric(typeof(UpdateServiceAsync<>)).As(typeof(IUpdateServiceAsync<>));

            builder.RegisterGeneric(typeof(CreateService<,>)).As(typeof(ICreateService<,>));
            builder.RegisterGeneric(typeof(CreateSetupService<,>)).As(typeof(ICreateSetupService<,>));
            builder.RegisterGeneric(typeof(DetailService<,>)).As(typeof(IDetailService<,>));
            builder.RegisterGeneric(typeof(ListService<,>)).As(typeof(IListService<,>));
            builder.RegisterGeneric(typeof(UpdateService<,>)).As(typeof(IUpdateService<,>));
            builder.RegisterGeneric(typeof(UpdateSetupService<,>)).As(typeof(IUpdateSetupService<,>));

            builder.RegisterGeneric(typeof(CreateServiceAsync<,>)).As(typeof(ICreateServiceAsync<,>));
            builder.RegisterGeneric(typeof(CreateSetupServiceAsync<,>)).As(typeof(ICreateSetupServiceAsync<,>));
            builder.RegisterGeneric(typeof(DetailServiceAsync<,>)).As(typeof(IDetailServiceAsync<,>));
            builder.RegisterGeneric(typeof(UpdateServiceAsync<,>)).As(typeof(IUpdateServiceAsync<,>));
            builder.RegisterGeneric(typeof(UpdateSetupServiceAsync<,>)).As(typeof(IUpdateSetupServiceAsync<,>));

        }

    }
}
