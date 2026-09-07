//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpClientGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag.CodeGeneration.CSharp.Models;

namespace NSwag.CodeGeneration.CSharp
{
    /// <summary>Generates the CSharp service client code. </summary>
    public class CSharpClientGenerator : CSharpGeneratorBase
    {
        private readonly OpenApiDocument _document;
        private readonly List<CSharpOperationModel> _collectedOperations = [];

        /// <summary>Initializes a new instance of the <see cref="CSharpClientGenerator" /> class.</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="settings">The settings.</param>
        /// <exception cref="ArgumentNullException"><paramref name="document" /> is <see langword="null" />.</exception>
        public CSharpClientGenerator(OpenApiDocument document, CSharpClientGeneratorSettings settings)
            : this(document, settings, CreateResolverWithExceptionSchema(settings.CSharpGeneratorSettings, document))
        {
        }

        /// <summary>Initializes a new instance of the <see cref="CSharpClientGenerator" /> class.</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="resolver">The resolver.</param>
        /// <exception cref="ArgumentNullException"><paramref name="document" /> is <see langword="null" />.</exception>
        public CSharpClientGenerator(OpenApiDocument document, CSharpClientGeneratorSettings settings, CSharpTypeResolver resolver)
            : base(document, settings, resolver)
        {
            Settings = settings;
            _document = document ?? throw new ArgumentNullException(nameof(document));

            ValidateAotSettings(settings);
        }

        private static void ValidateAotSettings(CSharpClientGeneratorSettings settings)
        {
            if (!settings.GenerateJsonSerializerContext)
            {
                return;
            }

            if (settings.CSharpGeneratorSettings.JsonLibrary != CSharpJsonLibrary.SystemTextJson)
            {
                throw new InvalidOperationException(
                    "GenerateJsonSerializerContext requires JsonLibrary = SystemTextJson. " +
                    "Set CSharpGeneratorSettings.JsonLibrary = CSharpJsonLibrary.SystemTextJson, or disable GenerateJsonSerializerContext.");
            }

            if (settings.CSharpGeneratorSettings.JsonLibraryVersion < 8.0m)
            {
                throw new InvalidOperationException(
                    "GenerateJsonSerializerContext requires JsonLibraryVersion >= 8.0 (TypeInfoResolverChain and the source generator are .NET 8+). " +
                    "Set CSharpGeneratorSettings.JsonLibraryVersion = 8.0m (or higher), or disable GenerateJsonSerializerContext.");
            }

            if (settings.CSharpGeneratorSettings.JsonPolymorphicSerializationStyle != CSharpJsonPolymorphicSerializationStyle.SystemTextJson)
            {
                throw new InvalidOperationException(
                    "GenerateJsonSerializerContext requires JsonPolymorphicSerializationStyle = SystemTextJson. " +
                    "The NJsonSchema polymorphic discriminator emits a reflection-based JsonInheritanceConverter that is not AOT-safe. " +
                    "Set CSharpGeneratorSettings.JsonPolymorphicSerializationStyle = CSharpJsonPolymorphicSerializationStyle.SystemTextJson.");
            }

            if (settings.CSharpGeneratorSettings.GenerateJsonMethods)
            {
                throw new InvalidOperationException(
                    "GenerateJsonSerializerContext is incompatible with GenerateJsonMethods because the generated ToJson()/FromJson() methods " +
                    "call the reflection-based JsonSerializer overloads. Disable GenerateJsonMethods.");
            }
        }

        /// <summary>Gets or sets the generator settings.</summary>
        public CSharpClientGeneratorSettings Settings { get; }

        /// <summary>Gets the base settings.</summary>
        public override ClientGeneratorBaseSettings BaseSettings => Settings;

        /// <summary>Generates the client class.</summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="controllerClassName">Name of the controller class.</param>
        /// <param name="operations">The operations.</param>
        /// <returns>The code.</returns>
        protected override IEnumerable<CodeArtifact> GenerateClientTypes(string controllerName, string controllerClassName, IEnumerable<CSharpOperationModel> operations)
        {
            var operationsList = operations as IList<CSharpOperationModel> ?? operations.ToList();

            if (Settings.GenerateJsonSerializerContext)
            {
                _collectedOperations.AddRange(operationsList);
            }

            var exceptionSchema = (Resolver as CSharpTypeResolver)?.ExceptionSchema;

            var model = new CSharpClientTemplateModel(controllerName, controllerClassName, operationsList, exceptionSchema, _document, Settings);
            if (model.HasOperations)
            {
                if (model.GenerateClientInterfaces && !model.SuppressClientInterfacesOutput)
                {
                    var interfaceTemplate = Settings.CSharpGeneratorSettings.TemplateFactory.CreateTemplate("CSharp", "Client.Interface", model);
                    yield return new CodeArtifact(model.Class, CodeArtifactType.Class, CodeArtifactLanguage.CSharp, CodeArtifactCategory.Contract, interfaceTemplate);
                }

                if (!model.SuppressClientClassesOutput)
                {
                    var classTemplate = Settings.CSharpGeneratorSettings.TemplateFactory.CreateTemplate("CSharp", "Client.Class", model);
                    yield return new CodeArtifact(model.Class, CodeArtifactType.Class, CodeArtifactLanguage.CSharp, CodeArtifactCategory.Client, classTemplate);
                }
            }
        }

        /// <summary>Generates the file, populating the AOT-mode JSON serializer context type list when enabled.</summary>
        /// <param name="clientTypes">The rendered client artifacts.</param>
        /// <param name="dtoTypes">The rendered DTO artifacts.</param>
        /// <param name="outputType">The output type.</param>
        /// <returns>The rendered file.</returns>
        protected override string GenerateFile(IEnumerable<CodeArtifact> clientTypes, IEnumerable<CodeArtifact> dtoTypes, ClientGeneratorOutputType outputType)
        {
            if (!Settings.GenerateJsonSerializerContext)
            {
                return base.GenerateFile(clientTypes, dtoTypes, outputType);
            }

            var model = new CSharpFileTemplateModel(clientTypes, dtoTypes, outputType, _document, Settings, this, (CSharpTypeResolver)Resolver)
            {
                JsonSerializableTypes = JsonSerializableTypeCollector.Collect(_collectedOperations)
            };

            var template = Settings.CSharpGeneratorSettings.TemplateFactory.CreateTemplate("CSharp", "File", model);
            return template.Render();
        }

        /// <summary>Creates an operation model.</summary>
        /// <param name="operation">The operation.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>The operation model.</returns>
        protected override CSharpOperationModel CreateOperationModel(OpenApiOperation operation, ClientGeneratorBaseSettings settings)
        {
            return new CSharpOperationModel(operation, (CSharpGeneratorBaseSettings)settings, this, (CSharpTypeResolver)Resolver);
        }
    }
}
