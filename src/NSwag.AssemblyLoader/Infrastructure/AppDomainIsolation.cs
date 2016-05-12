//-----------------------------------------------------------------------
// <copyright file="AppDomainIsolation.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace NSwag.CodeGeneration.Infrastructure
{
    internal sealed class AppDomainIsolation<T> : IDisposable where T : MarshalByRefObject
    {
        /// <exception cref="ArgumentNullException"><paramref name="assemblyDirectory"/> is <see langword="null" />.</exception>
        public AppDomainIsolation(string assemblyDirectory, string assemblyConfiguration)
        {
            if (string.IsNullOrEmpty(assemblyDirectory))
                throw new ArgumentNullException(nameof(assemblyDirectory));

            using (var transformer = new AssemblyConfigurationFileTransformer())
            {
                var setup = new AppDomainSetup
                {
                    ShadowCopyFiles = "true",
                    ApplicationBase = assemblyDirectory,
                    ConfigurationFile = !string.IsNullOrEmpty(assemblyConfiguration) ? assemblyConfiguration : transformer.GetConfigurationPath(assemblyDirectory)
                };

                Domain = AppDomain.CreateDomain("AppDomainIsolation:" + Guid.NewGuid(), null, setup);
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
}
