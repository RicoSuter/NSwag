//-----------------------------------------------------------------------
// <copyright file="NSwagSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NConsole;
using System;
using System.Linq;
using System.Reflection;

namespace NSwag.Commands.Generation
{
    /// <inheritdoc />
    public abstract class OpenApiGeneratorCommandBase : IsolatedSwaggerOutputCommandBase
    {
        [Argument(Name = "ServiceHost", IsRequired = false, Description = "Overrides the service host of the web service (optional, use '.' to remove the hostname).")]
        public string ServiceHost { get; set; }

        [Argument(Name = "ServiceBasePath", IsRequired = false, Description = "The basePath of the Swagger specification (optional).")]
        public string ServiceBasePath { get; set; }

        [Argument(Name = "ServiceSchemes", IsRequired = false, Description = "Overrides the allowed schemes of the web service (optional, comma separated, 'http', 'https', 'ws', 'wss').")]
        public string[] ServiceSchemes { get; set; } = new string[0];

        [Argument(Name = "DocumentTemplate", IsRequired = false, Description = "Specifies the Swagger document template (may be a path or JSON, default: none).")]
        public string DocumentTemplate { get; set; }

        [Argument(Name = "DocumentProcessors", IsRequired = false, Description = "The document processor type names in the form 'assemblyName:fullTypeName' or 'fullTypeName'.")]
        public string[] DocumentProcessorTypes { get; set; } = new string[0];

        [Argument(Name = "OperationProcessors", IsRequired = false, Description = "The operation processor type names in the form 'assemblyName:fullTypeName' or 'fullTypeName' or ':assemblyName:fullTypeName' or ':fullTypeName'. Begin name with ':' to prepend processors (required when used to filter out other operations).")]
        public string[] OperationProcessorTypes { get; set; } = new string[0];

        [Argument(Name = "TypeNameGenerator", IsRequired = false, Description = "The custom ITypeNameGenerator implementation type in the form 'assemblyName:fullTypeName' or 'fullTypeName'.")]
        public string TypeNameGeneratorType { get; set; }

        [Argument(Name = "SchemaNameGenerator", IsRequired = false, Description = "The custom ISchemaNameGenerator implementation type in the form 'assemblyName:fullTypeName' or 'fullTypeName'.")]
        public string SchemaNameGeneratorType { get; set; }

        [Argument(Name = "SerializerSettings", IsRequired = false, Description = "The custom JsonSerializerSettings implementation type in the form 'assemblyName:fullTypeName' or 'fullTypeName'.")]
        public string SerializerSettingsType { get; set; }

        [Argument(Name = "DocumentName", IsRequired = false, Description = "The document name to use in SwaggerDocumentProvider (default: v1).")]
        public string DocumentName { get; set; } = "v1";

        [Argument(Name = "AspNetCoreEnvironment", IsRequired = false, Description = "Sets the ASPNETCORE_ENVIRONMENT if provided (default: empty).")]
        public string AspNetCoreEnvironment { get; set; }

        [Argument(Name = "CreateWebHostBuilderMethod", IsRequired = false, Description = "The CreateWebHostBuilder method in the form 'assemblyName:fullTypeName.methodName' or 'fullTypeName.methodName'.")]
        public string CreateWebHostBuilderMethod { get; set; }

        [Argument(Name = "Startup", IsRequired = false, Description = "The Startup class type in the form 'assemblyName:fullTypeName' or 'fullTypeName'.")]
        public string StartupType { get; set; }

        protected IServiceProvider GetServiceProvider(AssemblyLoader.AssemblyLoader assemblyLoader)
        {
            if (!string.IsNullOrEmpty(CreateWebHostBuilderMethod))
            {
                // Load configured CreateWebHostBuilder method from program type
                var segments = CreateWebHostBuilderMethod.Split('.');

                var programTypeName = string.Join(".", segments.Take(segments.Length - 1));
                var programType = assemblyLoader.GetType(programTypeName) ??
                    throw new InvalidOperationException("The Program class could not be determined.");

                var method = programType.GetRuntimeMethod(segments.Last(), new[] { typeof(string[]) });
                if (method != null)
                {
                    return ((IWebHostBuilder)method.Invoke(null, new object[] { new string[0] })).Build().Services;
                }
                else
                {
                    method = programType.GetRuntimeMethod(segments.Last(), new Type[0]);
                    if (method != null)
                    {
                        return ((IWebHostBuilder)method.Invoke(null, new object[0])).Build().Services;
                    }
                    else
                    {
                        throw new InvalidOperationException("The CreateWebHostBuilderMethod '" + CreateWebHostBuilderMethod + "' could not be found.");
                    }
                }
            }
            else if (!string.IsNullOrEmpty(StartupType))
            {
                // Load configured startup type (obsolete)
                var startupType = assemblyLoader.GetType(StartupType);
                return WebHost.CreateDefaultBuilder().UseStartup(startupType).Build().Services;
            }
            else
            {
                var assemblies = LoadAssemblies(AssemblyPaths, assemblyLoader);
                var firstAssembly = assemblies.FirstOrDefault() ?? throw new InvalidOperationException("No assembly are be loaded from AssemblyPaths.");
                return ServiceProviderResolver.GetServiceProvider(firstAssembly);
            }
        }
    }
}
