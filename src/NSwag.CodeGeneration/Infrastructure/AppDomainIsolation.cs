//-----------------------------------------------------------------------
// <copyright file="AppDomainIsolation.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

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
                ConfigurationFile = GetTransformedConfigurationPath(assemblyDirectory)
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

        private string GetTransformedConfigurationPath(string assemblyDirectory)
        {
            var configPath = GetConfigurationPath(assemblyDirectory);
            if (configPath != null)
            {
                var content = File.ReadAllText(configPath);
                configPath = configPath + ".nswag";

                // Transform Newtonsoft.Json binding redirect so that all code uses the same JSON classes
                content = Regex.Replace(content, "<assemblyIdentity name=\"Newtonsoft.Json\"((.|\n|\r)*?)</dependentAssembly>",
                    match => Regex.Replace(match.Value, "oldVersion=\"(.*?)\"", "oldVersion=\"0.0.0.0-9999.0.0.0\""),
                    RegexOptions.Singleline);

                File.WriteAllText(configPath, content);
                return configPath;
            }
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "App.config");
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
