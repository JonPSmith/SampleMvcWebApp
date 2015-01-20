#region licence
// The MIT License (MIT)
// 
// Filename: ServiceLayerModule.cs
// Date Created: 2014/05/20
// 
// Copyright (c) 2014 Jon Smith (www.selectiveanalytics.com & www.thereformedprogrammer.net)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

using Autofac;
using DataLayer.Startup;
using GenericServices;

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

            //---------------------------
            //Register service layer: autowire all 
            builder.RegisterAssemblyTypes(GetType().Assembly).AsImplementedInterfaces();

            //and register the GenericServices assembly
            builder.RegisterAssemblyTypes(typeof(IListService).Assembly).AsImplementedInterfaces();

        }

    }
}
