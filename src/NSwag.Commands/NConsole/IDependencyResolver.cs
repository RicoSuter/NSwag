using System;

namespace NConsole
{
    /// <summary>Interface to resolve a dependency.</summary>
    public interface IDependencyResolver
    {
        /// <summary>Resolves the service of the given type.</summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        object GetService(Type serviceType);
    }
}