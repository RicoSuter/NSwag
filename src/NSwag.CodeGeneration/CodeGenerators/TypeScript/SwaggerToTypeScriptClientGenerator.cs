//-----------------------------------------------------------------------
// <copyright file="SwaggerToTypeScriptClientGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using NJsonSchema;
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag.CodeGeneration.CodeGenerators.Models;
using NSwag.CodeGeneration.CodeGenerators.TypeScript.Models;

namespace NSwag.CodeGeneration.CodeGenerators.TypeScript
{
    /// <summary>Generates the CSharp service client code. </summary>
    public class SwaggerToTypeScriptClientGenerator : ClientGeneratorBase
    {
        private readonly SwaggerService _service;
        private readonly TypeScriptTypeResolver _resolver;

        /// <summary>Initializes a new instance of the <see cref="SwaggerToTypeScriptClientGenerator" /> class.</summary>
        /// <param name="service">The service.</param>
        /// <param name="settings">The settings.</param>
        /// <exception cref="System.ArgumentNullException">service</exception>
        /// <exception cref="ArgumentNullException"><paramref name="service" /> is <see langword="null" />.</exception>
        public SwaggerToTypeScriptClientGenerator(SwaggerService service, SwaggerToTypeScriptClientGeneratorSettings settings)
            : this(service, settings, new TypeScriptTypeResolver(settings.TypeScriptGeneratorSettings, service))
        {

        }

        /// <summary>Initializes a new instance of the <see cref="SwaggerToTypeScriptClientGenerator" /> class.</summary>
        /// <param name="service">The service.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="resolver">The resolver.</param>
        /// <exception cref="System.ArgumentNullException">service</exception>
        /// <exception cref="ArgumentNullException"><paramref name="service" /> is <see langword="null" />.</exception>
        public SwaggerToTypeScriptClientGenerator(SwaggerService service, SwaggerToTypeScriptClientGeneratorSettings settings, TypeScriptTypeResolver resolver)
            : base(resolver, settings.CodeGeneratorSettings)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            Settings = settings;

            _service = service;
            _resolver = resolver;

            foreach (var definition in _service.Definitions.Where(p => string.IsNullOrEmpty(p.Value.TypeNameRaw)))
                definition.Value.TypeNameRaw = definition.Key;

            _resolver.AddSchemas(_service.Definitions);
        }

        /// <summary>Gets or sets the generator settings.</summary>
        public SwaggerToTypeScriptClientGeneratorSettings Settings { get; set; }

        /// <summary>Gets the language.</summary>
        protected override string Language => "TypeScript";

        /// <summary>Generates the file.</summary>
        /// <returns>The file contents.</returns>
        public override string GenerateFile()
        {
            return GenerateFile(_service, ClientGeneratorOutputType.Full);
        }

        /// <summary>Resolves the type of the parameter.</summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The parameter type name.</returns>
        protected override string ResolveParameterType(SwaggerParameter parameter)
        {
            var schema = parameter.ActualSchema;
            if (schema.Type == JsonObjectType.File)
            {
                if (parameter.CollectionFormat == SwaggerParameterCollectionFormat.Multi && !schema.Type.HasFlag(JsonObjectType.Array))
                    return "FileParameter[]";

                return "FileParameter";
            }

            return base.ResolveParameterType(parameter);
        }

        internal override ClientGeneratorBaseSettings BaseSettings => Settings;

        internal override string GenerateFile(string clientCode, IEnumerable<string> clientClasses, ClientGeneratorOutputType outputType)
        {
            var model = new FileTemplateModel(_service, clientCode, clientClasses, Settings, _resolver);
            var template = BaseSettings.CodeGeneratorSettings.TemplateFactory.CreateTemplate("TypeScript", "File", model);
            return template.Render();
        }

        internal override string GenerateClientClass(string controllerName, string controllerClassName, IList<OperationModel> operations, ClientGeneratorOutputType outputType)
        {
            UpdateUseDtoClassAndDataConversionCodeProperties(operations);

            var model = new ClientTemplateModel(GetClassName(controllerClassName), operations, _service, Settings);
            var template = Settings.CreateTemplate(model);
            var code = template.Render();

            return AppendExtensionClassIfNecessary(controllerClassName, code);
        }

        private string AppendExtensionClassIfNecessary(string controllerName, string code)
        {
            if (Settings.TypeScriptGeneratorSettings.ExtendedClasses?.Contains(controllerName) == true)
            {
                var extensionCode = Settings.TypeScriptGeneratorSettings.ProcessedExtensionCode;
                return extensionCode.Classes.ContainsKey(controllerName)
                    ? code + "\n\n" + extensionCode.Classes[controllerName]
                    : code;
            }
            return code;
        }

        internal override string GetExceptionType(SwaggerOperation operation)
        {
            if (operation.Responses.Count(r => !HttpUtilities.IsSuccessStatusCode(r.Key)) == 0)
                return "string";

            return string.Join(" | ", operation.Responses
                .Where(r => !HttpUtilities.IsSuccessStatusCode(r.Key) && r.Value.Schema != null)
                .Select(r => GetType(r.Value.ActualResponseSchema, r.Value.IsNullable(Settings.CodeGeneratorSettings.NullHandling), "Exception"))
                .Concat(new[] { "string" }));
        }

        internal override string GetResultType(SwaggerOperation operation)
        {
            var response = GetSuccessResponse(operation);
            if (response?.Schema == null)
                return "void";

            return GetType(response.ActualResponseSchema, response.IsNullable(Settings.CodeGeneratorSettings.NullHandling), "Response");
        }

        internal override string GetType(JsonSchema4 schema, bool isNullable, string typeNameHint)
        {
            if (schema == null)
                return "void";

            if (schema.ActualSchema.Type == JsonObjectType.File)
                return "any";

            if (schema.ActualSchema.IsAnyType || schema.ActualSchema.Type == JsonObjectType.File)
                return "any";

            return _resolver.Resolve(schema.ActualSchema, isNullable, typeNameHint);
        }

        private string GetClassName(string className)
        {
            if (Settings.TypeScriptGeneratorSettings.ExtendedClasses?.Contains(className) == true)
                return className + "Base";

            return className;
        }

        private void UpdateUseDtoClassAndDataConversionCodeProperties(IEnumerable<OperationModel> operations)
        {
            foreach (var operation in operations)
            {
                foreach (var parameter in operation.Parameters)
                {
                    if (parameter.IsDictionary)
                    {
                        if (parameter.Schema.AdditionalPropertiesSchema != null)
                        {
                            var itemTypeName = _resolver.Resolve(parameter.Schema.AdditionalPropertiesSchema, false, string.Empty);
                            parameter.UseDtoClass = Settings.TypeScriptGeneratorSettings.GetTypeStyle(itemTypeName) != TypeScriptTypeStyle.Interface &&
                                _resolver.HasTypeGenerator(itemTypeName);
                        }
                    }
                    else if (parameter.IsArray)
                    {
                        if (parameter.Schema.Item != null)
                        {
                            var itemTypeName = _resolver.Resolve(parameter.Schema.Item, false, string.Empty);
                            parameter.UseDtoClass = Settings.TypeScriptGeneratorSettings.GetTypeStyle(itemTypeName) != TypeScriptTypeStyle.Interface &&
                                _resolver.HasTypeGenerator(itemTypeName);
                        }
                    }
                    else
                        parameter.UseDtoClass = Settings.TypeScriptGeneratorSettings.GetTypeStyle(parameter.Type) != TypeScriptTypeStyle.Interface &&
                            _resolver.HasTypeGenerator(parameter.Type);
                }

                foreach (var response in operation.Responses.Where(r => r.HasType))
                {
                    response.UseDtoClass = Settings.TypeScriptGeneratorSettings.GetTypeStyle(response.Type) != TypeScriptTypeStyle.Interface;
                    response.DataConversionCode = DataConversionGenerator.RenderConvertToClassCode(new DataConversionParameters
                    {
                        Variable = "result" + response.StatusCode,
                        Value = "resultData" + response.StatusCode,
                        Schema = response.ActualResponseSchema,
                        IsPropertyNullable = response.IsNullable,
                        TypeNameHint = string.Empty,
                        Settings = Settings.TypeScriptGeneratorSettings,
                        Resolver = _resolver
                    });
                }

                if (operation.HasDefaultResponse && operation.DefaultResponse.HasType)
                {
                    operation.DefaultResponse.UseDtoClass = Settings.TypeScriptGeneratorSettings.GetTypeStyle(operation.DefaultResponse.Type) != TypeScriptTypeStyle.Interface;
                    operation.DefaultResponse.DataConversionCode = DataConversionGenerator.RenderConvertToClassCode(new DataConversionParameters
                    {
                        Variable = "result",
                        Value = "resultData",
                        Schema = operation.DefaultResponse.ActualResponseSchema,
                        IsPropertyNullable = operation.DefaultResponse.IsNullable,
                        TypeNameHint = string.Empty,
                        Settings = Settings.TypeScriptGeneratorSettings,
                        Resolver = _resolver
                    });
                }
            }
        }
    }
}
