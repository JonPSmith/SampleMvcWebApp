using System;
using GenericServices;
using ServiceLayer.TestActionService.Concrete;

namespace ServiceLayer.TestActionService
{
    public interface ICommsTestActionNoCancelEtc : IActionDefn<int,CommsTestActionData>, IDisposable
    {

    }
}
