using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SampleWebApp.Infrastructure;

namespace SampleWebApp.ActionProgress
{
    public static class RunnerSetupFactory<TTaskI>
    {
        /// <summary>
        /// This factory will create a RunnerSetup for an interface class that is intellisense friendly
        /// </summary>
        /// <param name="dto">data that the action needs</param>
        /// <param name="successExitUrl">optional url to go to after the user pressed Finished (OK)</param>
        /// <returns>The json to sent to the action javascript</returns>
        public static JsonNetResult CreateRunnerAndReturnJsonNetResult<TData>(TData dto, string successExitUrl = null) where TData : class
        {
            var runner = new RunnerSetup<TData>(typeof (TTaskI), dto, new ActionConfig(successExitUrl));
            return new JsonNetResult
            {
                Data = runner
            };
        }

        /// <summary>
        /// This factory will create a RunnerSetup for an interface class that is intellisense friendly
        /// </summary>
        /// <typeparam name="TTaskI">The type of the action to be injected via DI (normally an interface)</typeparam>
        /// <param name="dto">data that the action needs</param>
        /// <param name="actionConfig">A user defined actionConfig</param>
        /// <returns>The json to sent to the action javascript</returns>
        public static JsonNetResult CreateRunnerAndReturnJsonNetResult<TData>(TData dto, ActionConfig actionConfig) where TData : class
        {
            var runner = new RunnerSetup<TData>(typeof(TTaskI), dto, actionConfig);
            return new JsonNetResult
            {
                Data = runner
            };
        }
    }
}