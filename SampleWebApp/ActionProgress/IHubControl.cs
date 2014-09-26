#region licence
// The MIT License (MIT)
// 
// Filename: IHubControl.cs
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
using System.Threading.Tasks;
using GenericServices.ActionComms;

namespace SampleWebApp.ActionProgress
{
    public interface IHubControl
    {
        /// <summary>
        /// This is the guid string of the created Action. Used for security and error checking
        /// </summary>
        string ActionGuid { get; }

        /// <summary>
        /// This is the connnectionId to talk directly to the user on a specific connection
        /// </summary>
        string UserConnectionId { get; }

        /// <summary>
        /// This runs the service as an async Task
        /// </summary>
        /// <param name="actionGuid"></param>
        /// <param name="userConnectionId"></param>
        /// <param name="hubSendMethods"></param>
        /// <returns>Final message, which says if there was a problem</returns>
        Task<ProgressMessage> RunActionAsync(string actionGuid, string userConnectionId, IActionHubSend hubSendMethods);

        /// <summary>
        /// This will cancel a running action
        /// </summary>
        /// <param name="actionGuid"></param>
        /// <returns>true if task was cancelled, or false if task wasn't running</returns>
        bool CancelRunningAction(string actionGuid);
    }
}
