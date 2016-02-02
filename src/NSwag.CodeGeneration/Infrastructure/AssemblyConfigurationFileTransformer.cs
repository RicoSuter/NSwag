//-----------------------------------------------------------------------
// <copyright file="AssemblyConfigurationFileTransformer.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace NSwag.CodeGeneration.Infrastructure
{
    internal class AssemblyConfigurationFileTransformer : IDisposable
    {
        private string _transformedConfiguationPath = null;

        public string GetConfigurationPath(string assemblyDirectory)
        {
            var configPath = GetOriginalConfigurationPath(assemblyDirectory);
            if (configPath != null)
            {
                var content = File.ReadAllText(configPath, Encoding.UTF8);
                configPath = configPath + ".nswagtemp";

                // Transform Newtonsoft.Json binding redirect so that all code uses the same JSON classes
                content = Regex.Replace(content, "<assemblyIdentity name=\"Newtonsoft.Json\"((.|\n|\r)*?)</dependentAssembly>",
                    match => Regex.Replace(match.Value, "oldVersion=\"(.*?)\"", "oldVersion=\"0.0.0.0-9999.0.0.0\""),
                    RegexOptions.Singleline);

                File.WriteAllText(configPath, content, Encoding.UTF8);
                _transformedConfiguationPath = configPath;
                return configPath;
            }

            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "App.config");
        }

        public void Dispose()
        {
            if (_transformedConfiguationPath != null)
            {
                File.Delete(_transformedConfiguationPath);
                _transformedConfiguationPath = null; 
            }
        }

        private string GetOriginalConfigurationPath(string assemblyDirectory)
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