using System;

namespace SampleWebApp.ActionProgress
{
    public interface IActionHubDependencyResolver : IDisposable
    {
        /// <summary>
        /// The Dependency Resolver resolves a type into a concrete instance 
        /// using the rules it has in its setup.
        /// </summary>
        /// <param name="typeToResolve"></param>
        /// <returns></returns>
        object Resolve(Type typeToResolve);
    }
}
