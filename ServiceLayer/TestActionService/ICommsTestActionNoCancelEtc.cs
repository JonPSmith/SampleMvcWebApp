using System;
using GenericServices.ActionComms;
using ServiceLayer.TestActionService.Concrete;

namespace ServiceLayer.TestActionService
{
    public interface ICommsTestActionNoCancelEtc : IActionCommsSync<int,CommsTestActionData>, IDisposable
    {

    }
}
