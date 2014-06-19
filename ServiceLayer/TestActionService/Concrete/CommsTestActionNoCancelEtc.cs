using System;
using System.Threading;
using GenericServices;
using GenericServices.Actions;
using GenericServices.Services;

namespace ServiceLayer.TestActionService.Concrete
{

    public class CommsTestActionNoCancelEtc : CommsTestActionBase, ICommsTestActionNoCancelEtc
    {

        /// <summary>
        /// This allows the action to configure what it supports, which then affects what the user sees
        /// Note: it must be a constant as it is read just after the action is created
        /// </summary>
        public override ActionFlags ActionConfig
        {
            get { return ActionFlags.ExitOnSuccess | ActionFlags.NoProgressSent | ActionFlags.NoMessagesSent | ActionFlags.CancelNotSupported; }
        }
    
    }
}