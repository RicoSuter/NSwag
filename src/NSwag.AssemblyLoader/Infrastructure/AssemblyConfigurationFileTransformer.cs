//-----------------------------------------------------------------------
// <copyright file="AssemblyConfigurationFileTransformer.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace NSwag.CodeGeneration.Infrastructure
{
    internal class AssemblyConfigurationFileTransformer : IDisposable
    {
        private const string EmptyConfig =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <runtime>
    <assemblyBinding xmlns=""urn:schemas-microsoft-com:asm.v1"">
    </assemblyBinding>
  </runtime>
</configuration>";

        private readonly string JsonNetAssemblyBinding =
@"<dependentAssembly>
  <assemblyIdentity name=""Newtonsoft.Json"" publicKeyToken=""30ad4fe6b2a6aeed"" culture=""neutral"" />
  <bindingRedirect oldVersion=""0.0.0.0-9999.0.0.0"" newVersion=""9.0.0.0"" />
</dependentAssembly>";

        private readonly string NSwagCoreAssemblyBinding =
@"<dependentAssembly>
  <assemblyIdentity name=""NSwag.Core"" publicKeyToken=""c2d88086e098d109"" culture=""neutral"" />
  <bindingRedirect oldVersion=""0.0.0.0-9999.0.0.0"" newVersion=""" + SwaggerService.ToolchainVersion + @""" />
</dependentAssembly>";

        private string _transformedConfigurationPath = null;

        public string GetConfigurationPath(string assemblyDirectory)
        {
            var configPath = TryFindConfigurationPath(assemblyDirectory);
            var content = configPath != null ? File.ReadAllText(configPath, Encoding.UTF8) : EmptyConfig;

            content = UpdateOrAddBindingRedirect(content, "Newtonsoft.Json", "9.0.0.0", JsonNetAssemblyBinding);
            content = UpdateOrAddBindingRedirect(content, "NSwag.Core", SwaggerService.ToolchainVersion, NSwagCoreAssemblyBinding);

            _transformedConfigurationPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".nswagtemp");
            File.WriteAllText(_transformedConfigurationPath, content, Encoding.UTF8);
            return _transformedConfigurationPath;
        }

        private string UpdateOrAddBindingRedirect(string content, string name, string newVersion, string assemblyBinding)
        {
            if (content.Contains(@"assemblyIdentity name=""" + name + @""""))
            {
                content = Regex.Replace(content, "<assemblyIdentity name=\"" + name + "\"((.|\n|\r)*?)</dependentAssembly>",
                    match => Regex.Replace(match.Value, "oldVersion=\"(.*?)\"", "oldVersion=\"0.0.0.0-9999.0.0.0\""),
                    RegexOptions.Singleline);

                content = Regex.Replace(content, "<assemblyIdentity name=\"" + name + "\"((.|\n|\r)*?)</dependentAssembly>",
                    match => Regex.Replace(match.Value, "newVersion=\"(.*?)\"", "newVersion=\"" + newVersion + "\""),
                    RegexOptions.Singleline);

                return content;
            }
            else
                return content.Replace(" </assemblyBinding>", assemblyBinding + "</assemblyBinding>");
        }

        public void Dispose()
        {
            if (_transformedConfigurationPath != null)
            {
                File.Delete(_transformedConfigurationPath);
                _transformedConfigurationPath = null;
            }
        }

        private string TryFindConfigurationPath(string assemblyDirectory)
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