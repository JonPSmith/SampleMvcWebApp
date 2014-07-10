using System;
using GenericServices.ActionComms;
using ServiceLayer.TestActionService.Concrete;

namespace ServiceLayer.TestActionService
{
    public interface ICommsTestActionExitOnSuccess : IActionCommsSync<int,CommsTestActionData>, IDisposable
    {

    }
}
