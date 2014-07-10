using System;
using GenericServices.ActionComms;
using ServiceLayer.TestActionService.Concrete;

namespace ServiceLayer.TestActionService
{
    public interface ICommsTestActionNormal : IActionCommsSync<int,CommsTestActionData>, IDisposable
    {

    }
}
