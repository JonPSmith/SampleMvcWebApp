using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SampleWebApp.Infrastructure;

namespace SampleWebApp.ActionProgress
{
    public static class RunnerSetupFactory<TActionI>
    {
        /// <summary>
        /// This factory will create a RunnerSetup for an interface class that is intellisense friendly
        /// </summary>
        /// <typeparam name="TActionI">The type of the action to be injected via DI (normally an interface)</typeparam>
        /// <typeparam name="TData">The type of the data handed to the task when it runs</typeparam>
        /// <param name="dto">data that the action needs</param>
        /// <returns>The json to sent to the action javascript</returns>
        public static JsonNetResult CreateRunnerAndReturnJsonNetResult<TData>(TData dto) where TData : class
        {
            var runner = new RunnerSetup<TData>(typeof (TActionI), dto);
            return new JsonNetResult
            {
                Data = runner
            };
        }

    }
}