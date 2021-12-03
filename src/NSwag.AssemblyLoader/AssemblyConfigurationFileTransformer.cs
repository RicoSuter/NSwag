//-----------------------------------------------------------------------
// <copyright file="AssemblyConfigurationFileTransformer.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if NETFRAMEWORK
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace NSwag.AssemblyLoader
{
    internal class AssemblyConfigurationFileTransformer
    {
        private const string EmptyConfig =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <runtime>
    <loadFromRemoteSources enabled=""true""/>
    <assemblyBinding xmlns=""urn:schemas-microsoft-com:asm.v1"">
      <dependentAssembly>
        <assemblyIdentity name=""Newtonsoft.Json"" publicKeyToken=""30ad4fe6b2a6aeed"" culture=""neutral""/>
        <bindingRedirect oldVersion=""0.0.0.0-65535.65535.65535.65535"" newVersion=""9.0.0.0""/>
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name=""System.Runtime"" publicKeyToken=""b03f5f7f11d50a3a"" culture=""neutral""/>
        <bindingRedirect oldVersion=""0.0.0.0-65535.65535.65535.65535"" newVersion=""4.0.0.0""/>
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name=""System.Reflection"" publicKeyToken=""b03f5f7f11d50a3a"" culture=""neutral""/>
        <bindingRedirect oldVersion=""0.0.0.0-65535.65535.65535.65535"" newVersion=""4.0.0.0""/>
      </dependentAssembly>
    </assemblyBinding>
  </runtime>
</configuration>";

        private const string AssemblyBinding =
@"<dependentAssembly>
  <assemblyIdentity name=""{name}"" publicKeyToken=""{publicKeyToken}"" culture=""neutral"" />
  <bindingRedirect oldVersion=""0.0.0.0-65535.65535.65535.65535"" newVersion=""{newVersion}"" />
</dependentAssembly>";

        public static byte[] GetConfigurationBytes(string assemblyDirectory, string configurationPath, IEnumerable<BindingRedirect> bindingRedirects)
        {
            configurationPath = !string.IsNullOrEmpty(configurationPath) ? configurationPath : TryFindConfigurationPath(assemblyDirectory);

            var content = !string.IsNullOrEmpty(configurationPath) ? File.ReadAllText(configurationPath, Encoding.UTF8) : EmptyConfig;
            foreach (var br in bindingRedirects)
            {
                content = UpdateOrAddBindingRedirect(content, br.Name, br.NewVersion, br.PublicKeyToken);
            }

            return Encoding.UTF8.GetBytes(content);
        }

        private static string UpdateOrAddBindingRedirect(string content, string name, string newVersion, string publicKeyToken)
        {
            if (content.Contains(@"assemblyIdentity name=""" + name + @""""))
            {
                content = Regex.Replace(content,
                    "<assemblyIdentity name=\"" + Regex.Escape(name) + "\"((.|\n|\r)*?)</dependentAssembly>",
                    match => Regex.Replace(match.Value, "oldVersion=\"(.*?)\"",
                        "oldVersion=\"0.0.0.0-65535.65535.65535.65535\""),
                    RegexOptions.Singleline);

                content = Regex.Replace(content,
                    "<assemblyIdentity name=\"" + Regex.Escape(name) + "\"((.|\n|\r)*?)</dependentAssembly>",
                    match => Regex.Replace(match.Value, "newVersion=\"(.*?)\"", "newVersion=\"" + newVersion + "\""),
                    RegexOptions.Singleline);

                return content;
            }
            else
            {
                return content.Replace("</assemblyBinding>", AssemblyBinding
                    .Replace("{name}", name)
                    .Replace("{publicKeyToken}", publicKeyToken)
                    .Replace("{newVersion}", newVersion) +
                    "</assemblyBinding>");
            }
        }

        private static string TryFindConfigurationPath(string assemblyDirectory)
        {
            var config = Path.Combine(assemblyDirectory, "App.config");
            if (File.Exists(config))
            {
                return config;
            }

            config = Path.Combine(assemblyDirectory, "Web.config");
            if (File.Exists(config))
            {
                return config;
            }

            config = Path.Combine(assemblyDirectory, "../Web.config");
            if (File.Exists(config))
            {
                return config;
            }

            return null;
        }
    }
}

#endif

public class BindingRedirect
{
    public string Name { get; }

    public string NewVersion { get; }

    public string PublicKeyToken { get; }

    public BindingRedirect(string name, string newVersion, string publicKeyToken)
    {
        Name = name;
        NewVersion = newVersion;
        PublicKeyToken = publicKeyToken;
    }

#if NETFRAMEWORK

    public BindingRedirect(string name, Type newVersionType, string publicKeyToken)
        : this(name, newVersionType.Assembly.GetName().Version.ToString(), publicKeyToken)
    {
    }

#endif
}
