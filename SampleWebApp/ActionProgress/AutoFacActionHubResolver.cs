using System;
using Autofac;

namespace SampleWebApp.ActionProgress
{
    public class AutoFacActionHubResolver : IActionHubDependencyResolver
    {

        private readonly ILifetimeScope _lifeTimeScope;

        public AutoFacActionHubResolver(IContainer container)
        {
            _lifeTimeScope = container.BeginLifetimeScope();
        }

        public object Resolve(Type typeToResolve)
        {
            return _lifeTimeScope.Resolve(typeToResolve);
        }

        public void Dispose()
        {
            _lifeTimeScope.Dispose();
        }
    }
}