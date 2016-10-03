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
using Newtonsoft.Json.Linq;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag.Annotations;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

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
  <assemblyIdentity name=""{name}"" publicKeyToken=""30ad4fe6b2a6aeed"" culture=""neutral"" />
  <bindingRedirect oldVersion=""0.0.0.0-9999.0.0.0"" newVersion=""{newVersion}"" />
</dependentAssembly>";

        private readonly string NSwagAssemblyBinding =
@"<dependentAssembly>
  <assemblyIdentity name=""{name}"" publicKeyToken=""c2d88086e098d109"" culture=""neutral"" />
  <bindingRedirect oldVersion=""0.0.0.0-9999.0.0.0"" newVersion=""{newVersion}"" />
</dependentAssembly>";

        private readonly string NJsonSchemaAssemblyBinding =
@"<dependentAssembly>
  <assemblyIdentity name=""{name}"" publicKeyToken=""c2f9c3bdfae56102"" culture=""neutral"" />
  <bindingRedirect oldVersion=""0.0.0.0-9999.0.0.0"" newVersion=""{newVersion}"" />
</dependentAssembly>";

        private string _transformedConfigurationPath = null;

        public string GetConfigurationPath(string assemblyDirectory, string configurationPath)
        {
            configurationPath = !string.IsNullOrEmpty(configurationPath) ? configurationPath : TryFindConfigurationPath(assemblyDirectory);
            var content = !string.IsNullOrEmpty(configurationPath) ? File.ReadAllText(configurationPath, Encoding.UTF8) : EmptyConfig;

            content = UpdateOrAddBindingRedirect(content, "Newtonsoft.Json", typeof(JToken), JsonNetAssemblyBinding);

            content = UpdateOrAddBindingRedirect(content, "NJsonSchema", typeof(JsonSchema4), NJsonSchemaAssemblyBinding);
            content = UpdateOrAddBindingRedirect(content, "NJsonSchema.CodeGeneration", typeof(CSharpGenerator), NJsonSchemaAssemblyBinding);

            content = UpdateOrAddBindingRedirect(content, "NSwag.Core", typeof(SwaggerService), NSwagAssemblyBinding);
            content = UpdateOrAddBindingRedirect(content, "NSwag.CodeGeneration", typeof(WebApiToSwaggerGenerator), NSwagAssemblyBinding);
            content = UpdateOrAddBindingRedirect(content, "NSwag.Annotations", typeof(SwaggerTagsAttribute), NSwagAssemblyBinding);

            _transformedConfigurationPath = Path.Combine(Path.GetTempPath(), "NSwag_" + Guid.NewGuid() + ".nswagtemp");
            File.WriteAllText(_transformedConfigurationPath, content, Encoding.UTF8);
            return _transformedConfigurationPath;
        }

        private string UpdateOrAddBindingRedirect(string content, string name, Type newVersionType, string assemblyBinding)
        {
            var newVersion = newVersionType.Assembly.GetName().Version.ToString();
            if (content.Contains(@"assemblyIdentity name=""" + name + @""""))
            {
                content = Regex.Replace(content, "<assemblyIdentity name=\"" + Regex.Escape(name) + "\"((.|\n|\r)*?)</dependentAssembly>",
                    match => Regex.Replace(match.Value, "oldVersion=\"(.*?)\"", "oldVersion=\"0.0.0.0-9999.0.0.0\""),
                    RegexOptions.Singleline);

                content = Regex.Replace(content, "<assemblyIdentity name=\"" + Regex.Escape(name) + "\"((.|\n|\r)*?)</dependentAssembly>",
                    match => Regex.Replace(match.Value, "newVersion=\"(.*?)\"", "newVersion=\"" + newVersion + "\""),
                    RegexOptions.Singleline);

                return content;
            }
            else
                return content.Replace("</assemblyBinding>", assemblyBinding
                    .Replace("{name}", name)
                    .Replace("{newVersion}", newVersion) + "</assemblyBinding>");
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