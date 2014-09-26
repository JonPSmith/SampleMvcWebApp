#region licence
// The MIT License (MIT)
// 
// Filename: CommsTestActionBase.cs
// Date Created: 2014/06/19
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
using System.Threading;
using GenericServices;
using GenericServices.ActionComms;
using GenericServices.Actions;
using GenericServices.Core;

namespace ServiceLayer.TestActionService.Concrete
{
    public abstract class CommsTestActionBase : ActionCommsBase
    {

        /// <summary>
        /// This allows the action to configure what it supports, which then affects what the user sees
        /// Note: it must be a constant as it is read just after the action is created
        /// </summary>
        public override ActionFlags ActionConfig
        {
            get { return ActionFlags.Normal; }
        }

        /// <summary>
        /// If true then the caller should call EF SubmitChanges if the method exited with status IsValid and
        /// it looks to see if the data part has a ICheckIfWarnings and if the WriteEvenIfWarning is false
        /// and there are warnings then it does not call SubmitChanges
        /// </summary>
        public override bool SubmitChangesOnSuccess { get { return false; } }

        public bool DisposeWasCalled { get; private set; }

        public ISuccessOrErrors<int> DoAction(IActionComms actionComms, CommsTestActionData dto)
        {
            ISuccessOrErrors<int> result = new SuccessOrErrors<int>();

            if (dto.Mode == TestServiceModes.ThrowExceptionOnStart)
                throw new Exception("Thrown exception at start.");

            DateTime startTime = DateTime.Now;

            ReportProgressAndCheckCancel(actionComms, 0,
                new ProgressMessage(ProgressMessageTypes.Info, "Action has started. Will run for {0:f1} seconds.", dto.NumIterations * dto.SecondsBetweenIterations));

            for (int i = 0; i < dto.NumIterations; i++)
            {
                if (dto.Mode == TestServiceModes.ThrowExceptionHalfWayThrough && (i + 1) / 2 >= dto.NumIterations / 2)
                    throw new Exception("Thrown exception half way through.");
                if (dto.Mode == TestServiceModes.ThrowOperationCanceledExceptionHalfWayThrough && (i + 1) / 2 >= dto.NumIterations / 2)
                    throw new OperationCanceledException();         //we simulate a cancel half way through work

                ReportProgress(actionComms, (i + 1) * 100 / (dto.NumIterations + 1),
                    new ProgressMessage(dto.Mode == TestServiceModes.RunButOutputErrors && (i % 2 == 0)
                        ? ProgressMessageTypes.Error
                        : ProgressMessageTypes.Info,
                        string.Format("Iteration {0} of {1} done.", i + 1, dto.NumIterations)));
                if (CancelPending(actionComms))
                {
                    if (!dto.FailToRespondToCancel)
                    {
                        //we will respond to cancel
                        if (dto.SecondsDelayToRespondingToCancel > 0)
                            //... but with an additional delay
                            Thread.Sleep((int)(dto.SecondsDelayToRespondingToCancel * 1000));

                        return result.AddSingleError("Cancelled by user.");
                    }
                }
                Thread.Sleep( (int)(dto.SecondsBetweenIterations * 1000));
            }

            if (dto.Mode == TestServiceModes.RunButOutputOneWarningAtEnd)
                result.AddWarning("The mode was set to RunButOutputOneWarningAtEnd.");

            if (dto.NumErrorsToExitWith > 0)
            {
                for (int i = 0; i < dto.NumErrorsToExitWith; i++)
                    result.AddSingleError(string.Format(
                        "Error {0}: You asked me to declare an error when finished.", i));
            }
            else
            {
                result.SetSuccessWithResult(dto.NumIterations,
                    string.Format("Have completed the action in {0:F2} seconds",
                    DateTime.Now.Subtract(startTime).TotalSeconds));
            }

            return result;
        }

        /// <summary>
        /// If the user wants something to be called at the end then adding IDisposable to the class ensures 
        /// that whatever happens Dispose on the task weill be called at the end
        /// </summary>
        public void Dispose()
        {
            DisposeWasCalled = true;
        }
    }
}
