using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataLayer.Startup;


namespace ServiceLayer.Startup
{
    /// <summary>
    /// This handles the initialisation of this layer and any other layers 
    /// </summary>
    public static class ServiceLayerInitialise
    {

        /// <summary>
        /// This should be called at Startup
        /// </summary>
        public static void InitialiseThis()
        {

            //Place any tasks that need initialising here

            DataLayerInitialise.InitialiseThis();

        }
    }
}
