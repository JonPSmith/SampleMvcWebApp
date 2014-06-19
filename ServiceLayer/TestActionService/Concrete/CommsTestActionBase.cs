using System;
using System.Threading;
using GenericServices;
using GenericServices.Actions;
using GenericServices.Services;

namespace ServiceLayer.TestActionService.Concrete
{
    public abstract class CommsTestActionBase : ActionBase
    {

        public bool DisposeWasCalled { get; private set; }

        public ISuccessOrErrors DoAction(IActionComms actionComms, CommsTestActionData data)
        {
            var result = new SuccessOrErrors();

            if (data.Mode == TestServiceModes.ThrowExceptionOnStart)
                throw new Exception("Thrown exception at start.");

            DateTime startTime = DateTime.Now;

            ReportProgressAndThrowExceptionIfCancelPending(actionComms, 0,
                new ProgressMessage(ProgressMessageTypes.Info, "Task has started. Will run for {0:f1} seconds.", data.NumIterations * data.SecondsBetweenIterations));

            for (int i = 0; i < data.NumIterations; i++)
            {
                bool halfWayThroughOrMore = (i + 1) / 2 >= data.NumIterations / 2;
                if (data.Mode == TestServiceModes.ThrowExceptionHalfWayThrough && halfWayThroughOrMore)
                    throw new Exception("Thrown exception half way through.");
                if (data.Mode == TestServiceModes.ThrowOperationCanceledExceptionHalfWayThrough && halfWayThroughOrMore)
                    throw new OperationCanceledException();         //we simulate a cancel half way through work

                ReportProgress(actionComms, (i + 1) * 100 / (data.NumIterations + 1),
                    new ProgressMessage(data.Mode == TestServiceModes.RunButOutputErrors && (i % 2 == 0)
                        ? ProgressMessageTypes.Error
                        : data.Mode == TestServiceModes.OutputButOutputWarnings && (i % 2 == 0)
                            ? ProgressMessageTypes.Warning
                            : ProgressMessageTypes.Info,
                        string.Format("Iteration {0} of {1} done.", i + 1, data.NumIterations)));
                if (CancelPending(actionComms))
                {
                    if (!data.FailToRespondToCancel)
                    {
                        //we will respond to cancel
                        if (data.SecondsDelayToRespondingToCancel > 0)
                            //... but with an additional delay
                            Thread.Sleep((int)(data.SecondsDelayToRespondingToCancel * 1000));
                    }
                    break;
                }

                Thread.Sleep((int)(data.SecondsBetweenIterations * 1000));
            }

            if (data.NumErrorsToExitWith > 0)
            {
                for (int i = 0; i < data.NumErrorsToExitWith; i++)
                    result.AddSingleError(string.Format(
                        "Error {0}: You asked me to declare an error when finished.", i));
            }
            else
            {
                result.SetSuccessMessage(string.Format("Have completed the task in {0:F2} seconds",
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
