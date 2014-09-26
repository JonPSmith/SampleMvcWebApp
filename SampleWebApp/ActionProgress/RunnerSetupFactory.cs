#region licence
// The MIT License (MIT)
// 
// Filename: RunnerSetupFactory.cs
// Date Created: 2014/06/04
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