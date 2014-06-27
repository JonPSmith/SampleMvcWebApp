using System;
using GenericServices;
using ServiceLayer.TestActionService.Concrete;

namespace ServiceLayer.TestActionService
{
    public interface ICommsTestActionNoCancelEtc : IActionSync<int,CommsTestActionData>, IDisposable
    {

    }
}
