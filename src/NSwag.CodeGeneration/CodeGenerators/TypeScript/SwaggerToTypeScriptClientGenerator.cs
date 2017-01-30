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
using NSwag.CodeGeneration.CodeGenerators.TypeScript.Models;

namespace NSwag.CodeGeneration.CodeGenerators.TypeScript
{
    /// <summary>Generates the CSharp service client code. </summary>
    public class SwaggerToTypeScriptClientGenerator : ClientGeneratorBase<TypeScriptOperationModel, TypeScriptParameterModel, TypeScriptResponseModel>
    {
        private readonly SwaggerDocument _document;
        private readonly TypeScriptTypeResolver _resolver;
        private readonly TypeScriptExtensionCode _extensionCode;

        /// <summary>Initializes a new instance of the <see cref="SwaggerToTypeScriptClientGenerator" /> class.</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="settings">The settings.</param>
        /// <exception cref="ArgumentNullException"><paramref name="document" /> is <see langword="null" />.</exception>
        public SwaggerToTypeScriptClientGenerator(SwaggerDocument document, SwaggerToTypeScriptClientGeneratorSettings settings)
            : this(document, settings, new TypeScriptTypeResolver(settings.TypeScriptGeneratorSettings, document))
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SwaggerToTypeScriptClientGenerator" /> class.</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="resolver">The resolver.</param>
        /// <exception cref="ArgumentNullException"><paramref name="document" /> is <see langword="null" />.</exception>
        public SwaggerToTypeScriptClientGenerator(SwaggerDocument document, SwaggerToTypeScriptClientGeneratorSettings settings, TypeScriptTypeResolver resolver)
            : base(resolver, settings.CodeGeneratorSettings)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            Settings = settings;

            _document = document;
            _resolver = resolver;
            _resolver.AddGenerators(_document.Definitions);
            _extensionCode = new TypeScriptExtensionCode(
                Settings.TypeScriptGeneratorSettings.ExtensionCode,
                (Settings.TypeScriptGeneratorSettings.ExtendedClasses ?? new string[] { }).Concat(new[] { Settings.ConfigurationClass }).ToArray(),
                new[] { Settings.ClientBaseClass });
        }

        /// <summary>Gets or sets the generator settings.</summary>
        public SwaggerToTypeScriptClientGeneratorSettings Settings { get; set; }

        /// <summary>Generates the file.</summary>
        /// <returns>The file contents.</returns>
        public override string GenerateFile()
        {
            return GenerateFile(_document, ClientGeneratorOutputType.Full);
        }

        /// <summary>Gets the base settings.</summary>
        public override ClientGeneratorBaseSettings BaseSettings => Settings;
        
        /// <summary>Gets the type.</summary>
        /// <param name="schema">The schema.</param>
        /// <param name="isNullable">if set to <c>true</c> [is nullable].</param>
        /// <param name="typeNameHint">The type name hint.</param>
        /// <returns>The type name.</returns>
        public override string GetTypeName(JsonSchema4 schema, bool isNullable, string typeNameHint)
        {
            if (schema == null)
                return "void";

            if (schema.ActualSchema.Type == JsonObjectType.File)
                return "any";

            if (schema.ActualSchema.IsAnyType || schema.ActualSchema.Type == JsonObjectType.File)
                return "any";

            return _resolver.Resolve(schema.ActualSchema, isNullable, typeNameHint);
        }

        /// <summary>Generates the file.</summary>
        /// <param name="clientCode">The client code.</param>
        /// <param name="clientClasses">The client classes.</param>
        /// <param name="outputType">Type of the output.</param>
        /// <returns>The code.</returns>
        protected override string GenerateFile(string clientCode, IEnumerable<string> clientClasses, ClientGeneratorOutputType outputType)
        {
            var model = new TypeScriptFileTemplateModel(clientCode, clientClasses, _document, _extensionCode, Settings, _resolver);
            var template = BaseSettings.CodeGeneratorSettings.TemplateFactory.CreateTemplate("TypeScript", "File", model);
            return template.Render();
        }

        /// <summary>Generates the client class.</summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="controllerClassName">Name of the controller class.</param>
        /// <param name="operations">The operations.</param>
        /// <param name="outputType">Type of the output.</param>
        /// <returns>The code.</returns>
        protected override string GenerateClientClass(string controllerName, string controllerClassName, IList<TypeScriptOperationModel> operations, ClientGeneratorOutputType outputType)
        {
            UpdateUseDtoClassAndDataConversionCodeProperties(operations);

            var model = new TypeScriptClientTemplateModel(GetClassName(controllerClassName), operations, _document, Settings);
            var template = Settings.CreateTemplate(model);
            var code = template.Render();

            return AppendExtensionClassIfNecessary(controllerClassName, code);
        }

        private string AppendExtensionClassIfNecessary(string controllerName, string code)
        {
            if (Settings.TypeScriptGeneratorSettings.ExtendedClasses?.Contains(controllerName) == true)
            {
                return _extensionCode.ExtensionClasses.ContainsKey(controllerName)
                    ? code + "\n\n" + _extensionCode.ExtensionClasses[controllerName]
                    : code;
            }
            return code;
        }

        /// <summary>Creates an operation model.</summary>
        /// <param name="operation"></param>
        /// <param name="settings">The settings.</param>
        /// <returns>The operation model.</returns>
        protected override TypeScriptOperationModel CreateOperationModel(SwaggerOperation operation, ClientGeneratorBaseSettings settings)
        {
            return new TypeScriptOperationModel(operation, settings, this, Resolver);
        }

        private string GetClassName(string className)
        {
            if (Settings.TypeScriptGeneratorSettings.ExtendedClasses?.Contains(className) == true)
                return className + "Base";

            return className;
        }

        private void UpdateUseDtoClassAndDataConversionCodeProperties(IEnumerable<TypeScriptOperationModel> operations)
        {
            // TODO: Remove this method => move to appropriate location

            foreach (var operation in operations)
            {
                foreach (var response in operation.Responses.Where(r => r.HasType))
                {
                    response.DataConversionCode = DataConversionGenerator.RenderConvertToClassCode(new DataConversionParameters
                    {
                        Variable = "result" + response.StatusCode,
                        Value = "resultData" + response.StatusCode,
                        Schema = response.ActualResponseSchema,
                        IsPropertyNullable = response.IsNullable,
                        TypeNameHint = string.Empty,
                        Settings = Settings.TypeScriptGeneratorSettings,
                        Resolver = _resolver, 
                        NullValue = TypeScriptNullValue.Null
                    });
                }

                if (operation.HasDefaultResponse && operation.DefaultResponse.HasType)
                {
                    operation.DefaultResponse.DataConversionCode = DataConversionGenerator.RenderConvertToClassCode(new DataConversionParameters
                    {
                        Variable = "result",
                        Value = "resultData",
                        Schema = operation.DefaultResponse.ActualResponseSchema,
                        IsPropertyNullable = operation.DefaultResponse.IsNullable,
                        TypeNameHint = string.Empty,
                        Settings = Settings.TypeScriptGeneratorSettings,
                        Resolver = _resolver,
                        NullValue = TypeScriptNullValue.Null
                    });
                }
            }
        }
    }
}
