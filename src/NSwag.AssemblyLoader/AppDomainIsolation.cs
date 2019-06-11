//-----------------------------------------------------------------------
// <copyright file="AppDomainIsolation.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;

#if NET451
using System.Diagnostics;
using Newtonsoft.Json.Linq;
#endif

namespace NSwag.AssemblyLoader
{
#if NET451

    public sealed class AppDomainIsolation<T> : IDisposable where T : AssemblyLoader
    {
        /// <exception cref="ArgumentNullException"><paramref name="assemblyDirectory"/> is <see langword="null" />.</exception>
        public AppDomainIsolation(string assemblyDirectory, string assemblyConfiguration, IEnumerable<BindingRedirect> bindingRedirects, IEnumerable<string> preloadedAssemblies)
        {
            if (string.IsNullOrEmpty(assemblyDirectory))
            {
                throw new ArgumentNullException(nameof(assemblyDirectory));
            }

            var configuration = AssemblyConfigurationFileTransformer.GetConfigurationBytes(
                assemblyDirectory, assemblyConfiguration, bindingRedirects);

            var setup = new AppDomainSetup();
            setup.ShadowCopyFiles = "true";
            setup.SetConfigurationBytes(configuration);

            Domain = AppDomain.CreateDomain("AppDomainIsolation:" + Guid.NewGuid(), null, setup);

            foreach (var pa in preloadedAssemblies)
            {
                Domain.Load(new AssemblyName { CodeBase = pa });
            }

            var type = typeof(T);
            try
            {
                Object = (T)Domain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
            }
            catch
            {
                Object = (T)Domain.CreateInstanceFromAndUnwrap(type.Assembly.Location, type.FullName);
            }
        }

        public AppDomain Domain { get; private set; }

        public T Object { get; }

        public void Dispose()
        {
            if (Domain != null)
            {
                AppDomain.Unload(Domain);
                Domain = null;
            }
        }
    }

#else

    public sealed class AppDomainIsolation<T> : IDisposable where T : AssemblyLoader, new()
    {
        /// <exception cref="ArgumentNullException"><paramref name="assemblyDirectory"/> is <see langword="null" />.</exception>
        public AppDomainIsolation(string assemblyDirectory, string assemblyConfiguration, IEnumerable<BindingRedirect> bindingRedirects, IEnumerable<Assembly> preloadedAssemblies)
        {
            Object = new T();

            foreach (var pa in preloadedAssemblies)
            {
                Object.Context.Assemblies[pa.GetName().Name] = pa;
            }
        }

        public T Object { get; }

        public void Dispose()
        {
        }
    }

#endif
}
