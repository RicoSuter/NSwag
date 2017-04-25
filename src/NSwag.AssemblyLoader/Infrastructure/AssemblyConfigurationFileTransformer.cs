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
using NSwag.Annotations;
using NSwag.SwaggerGeneration;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.CodeGeneration.Infrastructure
{
    internal class AssemblyConfigurationFileTransformer
    {
        private const string EmptyConfig =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <runtime>
    <assemblyBinding xmlns=""urn:schemas-microsoft-com:asm.v1"">
    </assemblyBinding>
  </runtime>
</configuration>";

        private const string JsonNetAssemblyBinding =
@"<dependentAssembly>
  <assemblyIdentity name=""{name}"" publicKeyToken=""30ad4fe6b2a6aeed"" culture=""neutral"" />
  <bindingRedirect oldVersion=""0.0.0.0-65535.65535.65535.65535"" newVersion=""{newVersion}"" />
</dependentAssembly>";

        private const string NSwagAssemblyBinding =
@"<dependentAssembly>
  <assemblyIdentity name=""{name}"" publicKeyToken=""c2d88086e098d109"" culture=""neutral"" />
  <bindingRedirect oldVersion=""0.0.0.0-65535.65535.65535.65535"" newVersion=""{newVersion}"" />
</dependentAssembly>";

        private const string NJsonSchemaAssemblyBinding =
@"<dependentAssembly>
  <assemblyIdentity name=""{name}"" publicKeyToken=""c2f9c3bdfae56102"" culture=""neutral"" />
  <bindingRedirect oldVersion=""0.0.0.0-65535.65535.65535.65535"" newVersion=""{newVersion}"" />
</dependentAssembly>";

        public static byte[] GetConfigurationBytes(string assemblyDirectory, string configurationPath)
        {
            configurationPath = !string.IsNullOrEmpty(configurationPath) ? configurationPath : TryFindConfigurationPath(assemblyDirectory);

            var content = !string.IsNullOrEmpty(configurationPath) ? File.ReadAllText(configurationPath, Encoding.UTF8) : EmptyConfig;
            content = UpdateOrAddBindingRedirect(content, "Newtonsoft.Json", typeof(JToken), JsonNetAssemblyBinding);
            content = UpdateOrAddBindingRedirect(content, "NJsonSchema", typeof(JsonSchema4), NJsonSchemaAssemblyBinding);
            content = UpdateOrAddBindingRedirect(content, "NSwag.Core", typeof(SwaggerDocument), NSwagAssemblyBinding);
            content = UpdateOrAddBindingRedirect(content, "NSwag.SwaggerGeneration", typeof(SwaggerJsonSchemaGenerator), NSwagAssemblyBinding);
            content = UpdateOrAddBindingRedirect(content, "NSwag.SwaggerGeneration.WebApi", typeof(WebApiToSwaggerGenerator), NSwagAssemblyBinding);
            content = UpdateOrAddBindingRedirect(content, "NSwag.Annotations", typeof(SwaggerTagsAttribute), NSwagAssemblyBinding);

            return Encoding.UTF8.GetBytes(content);
        }

        private static string UpdateOrAddBindingRedirect(string content, string name, Type newVersionType, string assemblyBinding)
        {
            var newVersion = newVersionType.Assembly.GetName().Version.ToString();
            if (content.Contains(@"assemblyIdentity name=""" + name + @""""))
            {
                content = Regex.Replace(content, "<assemblyIdentity name=\"" + Regex.Escape(name) + "\"((.|\n|\r)*?)</dependentAssembly>",
                    match => Regex.Replace(match.Value, "oldVersion=\"(.*?)\"", "oldVersion=\"0.0.0.0-65535.65535.65535.65535\""),
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

        private static string TryFindConfigurationPath(string assemblyDirectory)
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