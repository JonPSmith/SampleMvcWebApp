#region licence
// The MIT License (MIT)
// 
// Filename: RunnerSetup.cs
// Date Created: 2014/05/28
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
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using GenericServices;
using GenericServices.ActionComms;
using GenericServices.Core;
using SampleWebApp.Infrastructure;

namespace SampleWebApp.ActionProgress
{

    public class RunnerSetup<TData> where TData : class
    {
        private readonly Type _actionTypeI;
        private readonly Type _actionInterface;
        private readonly TData _data;
        private readonly bool _isAsync;
        private ISuccessOrErrors _status;

        /// <summary>
        /// This is the string holding a Guid for the created action. Used for for saving in dictionary and security checks
        /// </summary>
        public string ActionGuid { get; private set; }

        //------------------------------------------------------------
        //ctor.

        /// <summary>
        /// This is given the class containing the interface to create, with the data is passing in, which it checks
        /// </summary>
        /// <param name="actionTypeI">An interface of the action to be injected via DI</param>
        /// <param name="data">the argument in the call</param>
        public RunnerSetup(Type actionTypeI, TData data)
        {
            _actionTypeI = actionTypeI;
            //we check and extract the IActionDefn or IActionDefnAsync part of the TActionI
            if (!actionTypeI.IsInterface)
                throw new InvalidOperationException("The TActionI must be an interface to work properly.");
            _actionInterface = actionTypeI.GetInterfaces().FirstOrDefault();
            if (_actionInterface == null || !_actionInterface.IsGenericType)
                throw new InvalidOperationException("The interface must have IActionDefn<TOut,Tin> or IActionDefnAsync<TOut,Tin> as the first sub-interface.");

            _isAsync = _actionInterface.Name == typeof (IActionCommsAsync<,>).Name;
            if (!_isAsync && _actionInterface.Name != typeof(IActionCommsSync<,>).Name)
                throw new InvalidOperationException("The interface must have IActionDefn<TOut,Tin> or IActionDefnAsync<TOut,Tin> as the first sub-interface.");

            _data = data;
        }

        /// <summary>
        /// This decodes the action and the data type and does a copy if the data handed in is a GenericDTO.
        /// If all is well then it sets up the HubRunner ready to receive the start command from the JavaScript
        /// </summary>
        /// <returns>It returns either json with an errorDict in it if the copy didn't go well, or the ActionGuid if it did</returns>
        public JsonResult SetupAndReturnJsonResult(IGenericServicesDbContext db)
        {          
            //now we extract the Action TOut and TIn and decide if a copy is needed
            var actionDefnParts = _actionInterface.GetGenericArguments();
            var actionOutType = actionDefnParts[0];
            var actionInType = actionDefnParts[1];

            if (typeof (TData) == actionInType)
                return SetupHubRunner(actionOutType, actionInType, _data);

            //else we need to copy over the dto to the data
            var actionData = Activator.CreateInstance(actionInType);
            var copyDtoToDataMethod = _data.GetType().GetMethod(_isAsync ? "UpdateDataFromDtoAsync" : "UpdateDataFromDto", BindingFlags.Instance | BindingFlags.NonPublic);
            if (copyDtoToDataMethod == null)
                throw new InvalidOperationException("The data you provided didn't match the action and neither was it a GenericDto");
            if (db == null)
                throw new ArgumentNullException("db", "The data you provided was a generic dto, which needs IGenericServicesDbContext");

            _status = _isAsync
                ? (copyDtoToDataMethod.Invoke(_data, new object[] {db, _data, actionData}) as Task<ISuccessOrErrors>)
                    .ConfigureAwait(false).GetAwaiter().GetResult()
                : copyDtoToDataMethod.Invoke(_data, new object[] { db, _data, actionData}) as ISuccessOrErrors;

            if (_status.IsValid)
                return SetupHubRunner(actionOutType, actionInType, actionData);

            //else there has been some errors in the copy, so return an errorDict to the JavaScript
            return _status.ReturnErrorsAsJson(_data);
        }

        private JsonResult SetupHubRunner(Type actionOutType, Type actionInType, dynamic actionData)
        {

            //We assign a action ID for this action
            ActionGuid = Guid.NewGuid().ToString();

            //Setup the HubRunner in the hubs dictionary
            var typeArgs = new []{ actionOutType, actionInType };
            var constructed = typeof(HubRunner<,>).MakeGenericType(typeArgs);
            var hubRunnerInstance = Activator.CreateInstance(constructed, ActionGuid, _actionTypeI, actionData, _isAsync);

            ActionHub.SetActionRunner(hubRunnerInstance);

            return new JsonResult { Data = new { ActionGuid = ActionGuid}};
        }

    }
}