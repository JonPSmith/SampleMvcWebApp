using Autofac;
using DataLayer.Startup;
using GenericServices;
using GenericServices.Services;

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

            builder.RegisterGeneric(typeof(CreateService<,>)).As(typeof(ICreateService<,>));
            builder.RegisterGeneric(typeof(CreateSetupService<,>)).As(typeof(ICreateSetupService<,>));
            builder.RegisterGeneric(typeof(DetailService<,>)).As(typeof(IDetailService<,>));
            builder.RegisterGeneric(typeof(ListService<,>)).As(typeof(IListService<,>));
            builder.RegisterGeneric(typeof(UpdateService<,>)).As(typeof(IUpdateService<,>));
            builder.RegisterGeneric(typeof(UpdateSetupService<,>)).As(typeof(IUpdateSetupService<,>));

        }

    }
}
