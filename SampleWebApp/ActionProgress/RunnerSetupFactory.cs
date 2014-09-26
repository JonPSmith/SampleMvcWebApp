using System;
using System.Linq;
using System.Web.Mvc;
using GenericServices;

namespace SampleWebApp.ActionProgress
{
    public static class RunnerSetupFactory<TActionI>
    {
        /// <summary>
        /// This factory will create a RunnerSetup for an interface class that is intellisense friendly
        /// </summary>
        /// <typeparam name="TActionI">An interface of the action to be injected via DI</typeparam>
        /// <typeparam name="TData">The type of the data handed it. Can be the data that the action needs or a GenericDTo of some sort</typeparam>
        /// <param name="data">data to pass to the action. If a dto then it copies it over</param>
        /// <param name="db">optional. Needed if the input data is a dto, which will need copying</param>
        /// <returns>The json to sent to the action javascript</returns>
        public static JsonResult SetupRunner<TData>(TData data, IGenericServicesDbContext db = null) where TData : class
        {

            var runner = new RunnerSetup<TData>(typeof (TActionI), data);
            return runner.SetupAndReturnJsonResult(db);
        }

    }
}