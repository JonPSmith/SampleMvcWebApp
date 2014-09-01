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
        /// <param name="isAzure">true if working with azure database</param>
        /// <param name="canCreateDatabase">true if the database provider allows the app to drop/create a database</param>
        public static void InitialiseThis(bool isAzure, bool canCreateDatabase)
        {

            //Place any tasks that need initialising here

            DataLayerInitialise.InitialiseThis(isAzure, canCreateDatabase);

        }
    }
}
