using System;
using NUnit.Framework;
using SampleWebApp.ActionProgress;

namespace Tests.MocksAndFakes
{
    public class MockHubDependencyResolver : IActionHubDependencyResolver
    {

        public bool DisposeWasCalled { get; private set; }

        public bool ReturnNullAlways { get; set; }

        public void Dispose()
        {
            DisposeWasCalled = true;
        }

        public object Resolve(Type typeToResolve)
        {
            if (ReturnNullAlways)
                return null;

            if (typeToResolve != typeof(IEmptyTestAction))
                Assert.Fail("The MockHubDependencyResolver can only return a EmptyTestAction");

            return new EmptyTestAction();
        }
    }
}
