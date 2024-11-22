﻿//-----------------------------------------------------------------------
// <copyright file="SwaggerToTypeScriptClientGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag.CodeGeneration.TypeScript.Models;

namespace NSwag.CodeGeneration.TypeScript
{
    /// <summary>Generates the CSharp service client code. </summary>
    public class TypeScriptClientGenerator : ClientGeneratorBase<TypeScriptOperationModel, TypeScriptParameterModel, TypeScriptResponseModel>
    {
        private readonly OpenApiDocument _document;
        private readonly TypeScriptTypeResolver _resolver;
        private readonly TypeScriptExtensionCode _extensionCode;

        /// <summary>Initializes a new instance of the <see cref="TypeScriptClientGenerator" /> class.</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="settings">The settings.</param>
        /// <exception cref="ArgumentNullException"><paramref name="document" /> is <see langword="null" />.</exception>
        public TypeScriptClientGenerator(OpenApiDocument document, TypeScriptClientGeneratorSettings settings)
            : this(document, settings, new TypeScriptTypeResolver(settings.TypeScriptGeneratorSettings))
        {
        }

        /// <summary>Initializes a new instance of the <see cref="TypeScriptClientGenerator" /> class.</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="resolver">The resolver.</param>
        /// <exception cref="ArgumentNullException"><paramref name="document" /> is <see langword="null" />.</exception>
        public TypeScriptClientGenerator(OpenApiDocument document, TypeScriptClientGeneratorSettings settings, TypeScriptTypeResolver resolver)
            : base(document, settings.CodeGeneratorSettings, resolver)
        {
            Settings = settings;

            _document = document ?? throw new ArgumentNullException(nameof(document));
            _resolver = resolver;
            _resolver.RegisterSchemaDefinitions(_document.Definitions);

            _extensionCode = new TypeScriptExtensionCode(
                Settings.TypeScriptGeneratorSettings.ExtensionCode,
                [.. (Settings.TypeScriptGeneratorSettings.ExtendedClasses ?? []), Settings.ConfigurationClass],
                [Settings.ClientBaseClass]);
        }

        /// <summary>Gets or sets the generator settings.</summary>
        public TypeScriptClientGeneratorSettings Settings { get; set; }

        /// <summary>Gets the base settings.</summary>
        public override ClientGeneratorBaseSettings BaseSettings => Settings;

        /// <summary>Gets the type.</summary>
        /// <param name="schema">The schema.</param>
        /// <param name="isNullable">Specifies whether the type is nullable..</param>
        /// <param name="typeNameHint">The type name hint.</param>
        /// <returns>The type name.</returns>
        public override string GetTypeName(JsonSchema schema, bool isNullable, string typeNameHint)
        {
            if (schema == null)
            {
                return "void";
            }

            if (schema.ActualTypeSchema.IsBinary)
            {
                return GetBinaryResponseTypeName();
            }

            return _resolver.Resolve(schema.ActualSchema, isNullable, typeNameHint);
        }

        /// <summary>Gets the file response type name.</summary>
        /// <returns>The type name.</returns>
        public override string GetBinaryResponseTypeName()
        {
            return Settings.Template is not TypeScriptTemplate.JQueryCallbacks and not TypeScriptTemplate.JQueryPromises
                ? "FileResponse"
                : "any";
        }

        /// <summary>Generates the file.</summary>
        /// <param name="clientTypes">The client types.</param>
        /// <param name="dtoTypes">The DTO types.</param>
        /// <param name="outputType">Type of the output.</param>
        /// <returns>The code.</returns>
        protected override string GenerateFile(IEnumerable<CodeArtifact> clientTypes, IEnumerable<CodeArtifact> dtoTypes, ClientGeneratorOutputType outputType)
        {
            var model = new TypeScriptFileTemplateModel(clientTypes, dtoTypes, _document, _extensionCode, Settings, _resolver);
            var template = BaseSettings.CodeGeneratorSettings.TemplateFactory.CreateTemplate("TypeScript", "File", model);
            return template.Render();
        }

        /// <summary>Generates the client class.</summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="controllerClassName">Name of the controller class.</param>
        /// <param name="operations">The operations.</param>
        /// <returns>The code.</returns>
        protected override IEnumerable<CodeArtifact> GenerateClientTypes(string controllerName, string controllerClassName, IEnumerable<TypeScriptOperationModel> operations)
        {
            UpdateUseDtoClassAndDataConversionCodeProperties(operations);

            var model = new TypeScriptClientTemplateModel(controllerName, controllerClassName, operations, _extensionCode, _document, Settings);
            var template = Settings.CreateTemplate(model);
            yield return new CodeArtifact(model.Class, CodeArtifactType.Class, CodeArtifactLanguage.CSharp, CodeArtifactCategory.Client, template);
        }

        /// <summary>Generates all DTO types.</summary>
        /// <returns>The code artifact collection.</returns>
        protected override IEnumerable<CodeArtifact> GenerateDtoTypes()
        {
            var generator = new TypeScriptGenerator(_document, Settings.TypeScriptGeneratorSettings, _resolver);
            return generator.GenerateTypes(_extensionCode);
        }

        /// <summary>Creates an operation model.</summary>
        /// <param name="operation"></param>
        /// <param name="settings">The settings.</param>
        /// <returns>The operation model.</returns>
        protected override TypeScriptOperationModel CreateOperationModel(OpenApiOperation operation, ClientGeneratorBaseSettings settings)
        {
            return new TypeScriptOperationModel(operation, (TypeScriptClientGeneratorSettings)settings, this, Resolver);
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
                        Schema = response.ResolvableResponseSchema,
                        CheckNewableObject = response.IsNullable,
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
                        Variable = "result" + operation.DefaultResponse.StatusCode,
                        Value = "resultData" + operation.DefaultResponse.StatusCode,
                        Schema = operation.DefaultResponse.ResolvableResponseSchema,
                        CheckNewableObject = operation.DefaultResponse.IsNullable,
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
