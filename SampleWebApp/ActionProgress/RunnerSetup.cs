using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using SampleWebApp.Infrastructure;

namespace SampleWebApp.ActionProgress
{

    public class RunnerSetup<T> where T : class
    {
        /// <summary>
        /// This is the string holding a Guid for the created action. Used for for saving in dictionary and security checks
        /// </summary>
        public string ActionGuid { get; private set; }

        /// <summary>
        /// Holds the configuration to use for this action
        /// </summary>
        public ActionConfig Config { get; private set; }

        //------------------------------------------------------------
        //ctor.

        /// <summary>
        /// This is given the class containing the interface to create, with the data it needs
        /// </summary>
        /// <param name="actionType">The type of the action to be injected via DI (normally an interface)</param>
        /// <param name="args">arguments to hand to action</param>
        /// <param name="actionConfig">A configuration file for the action.</param>
        public RunnerSetup(Type actionType, T args, ActionConfig actionConfig)
        {

            //We assign a action ID for this action
            ActionGuid = Guid.NewGuid().ToString();
            Config = actionConfig ?? new ActionConfig();

            //Setup the HubRunner in the hubs dictionary
            ActionHub.SetActionRunner(new HubRunner<T>(ActionGuid, actionType, args));
        }
            
    }
}