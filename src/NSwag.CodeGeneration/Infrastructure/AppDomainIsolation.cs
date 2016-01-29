//-----------------------------------------------------------------------
// <copyright file="AppDomainIsolation.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.IO;

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

            var setup = new AppDomainSetup
            {
                ShadowCopyFiles = "true",
                ApplicationBase = assemblyDirectory, 
                ConfigurationFile = GetConfigurationPath(assemblyDirectory)
            };

            _domain = AppDomain.CreateDomain("AppDomainIsolation:" + Guid.NewGuid(), null, setup);
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

        public string GetConfigurationPath(string assemblyDirectory)
        {
            var config = Path.Combine(assemblyDirectory, "App.config");
            if (File.Exists(config))
                return config;

            config = Path.Combine(assemblyDirectory, "Web.config");
            if (File.Exists(config))
                return config;

            config = Path.Combine(assemblyDirectory, "../Web.config");
            if (File.Exists(config))
                return config;

            return null; 
        }
    }
}
