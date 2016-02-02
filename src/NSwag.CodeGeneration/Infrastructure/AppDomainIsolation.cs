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
        private AppDomain _domain;
        private readonly T _object;

        /// <exception cref="ArgumentNullException"><paramref name="assemblyDirectory"/> is <see langword="null" />.</exception>
        public AppDomainIsolation(string assemblyDirectory)
        {
            if (string.IsNullOrEmpty(assemblyDirectory))
                throw new ArgumentNullException("assemblyDirectory");

            using (var transformer = new AssemblyConfigurationFileTransformer())
            {
                var setup = new AppDomainSetup
                {
                    ShadowCopyFiles = "true",
                    ApplicationBase = assemblyDirectory,
                    ConfigurationFile = transformer.GetConfigurationPath(assemblyDirectory)
                };

                _domain = AppDomain.CreateDomain("AppDomainIsolation:" + Guid.NewGuid(), null, setup);
            }

            var type = typeof(T);
            try
            {
                _object = (T)_domain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
            }
            catch
            {
                _object = (T)_domain.CreateInstanceFromAndUnwrap(type.Assembly.Location, type.FullName);
            }
        }

        public T Object
        {
            get { return _object; }
        }

        public void Dispose()
        {
            if (_domain != null)
            {
                AppDomain.Unload(_domain);
                _domain = null;
            }
        }
    }
}
