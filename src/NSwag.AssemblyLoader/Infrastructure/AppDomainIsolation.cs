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
                    ConfigurationFile = transformer.GetConfigurationPath(assemblyDirectory, assemblyConfiguration)
                };

                Domain = AppDomain.CreateDomain("AppDomainIsolation:" + Guid.NewGuid(), null, setup);
            }

            var type = typeof(T);
            LoadToolchainAssemblies(type);
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

        private void LoadToolchainAssemblies(Type type)
        {
            var codeBaseDirectory = Path.GetDirectoryName(type.Assembly.CodeBase.Replace("file:///", string.Empty));
            Domain.Load(new AssemblyName { CodeBase = codeBaseDirectory + "\\Newtonsoft.Json.dll" });
            Domain.Load(new AssemblyName { CodeBase = codeBaseDirectory + "\\NJsonSchema.dll" });
            Domain.Load(new AssemblyName { CodeBase = codeBaseDirectory + "\\NJsonSchema.CodeGeneration.dll" });
            Domain.Load(new AssemblyName { CodeBase = codeBaseDirectory + "\\NSwag.Core.dll" });
            Domain.Load(new AssemblyName { CodeBase = codeBaseDirectory + "\\NSwag.Commands.dll" });
            Domain.Load(new AssemblyName { CodeBase = codeBaseDirectory + "\\NSwag.CodeGeneration.dll" });
        }
    }
}
