//-----------------------------------------------------------------------
// <copyright file="ClientGeneratorBaseSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration.OperationNameGenerators;

namespace NSwag.CodeGeneration
{
    /// <summary>Settings for the ClientGeneratorBase.</summary>
    public abstract class ClientGeneratorBaseSettings
    {
        /// <summary>Initializes a new instance of the <see cref="ClientGeneratorBaseSettings"/> class.</summary>
        protected ClientGeneratorBaseSettings()
        {
            GenerateClientClasses = true;
            GenerateDtoTypes = true;

            OperationNameGenerator = new MultipleClientsFromOperationIdOperationNameGenerator();
            ParameterNameGenerator = new DefaultParameterNameGenerator();

            GenerateResponseClasses = true;
            ResponseClass = "SwaggerResponse";
            CombinedClientClassName = "CombinedClient";
            CombinedClassConstructorAccess = "public";

            WrapResponseMethods = new string[0];
            ExcludedParameterNames = new string[0];
        }

        /// <summary>Gets the code generator settings.</summary>
        public abstract CodeGeneratorSettingsBase CodeGeneratorSettings { get; }

        /// <summary>Gets or sets the class name of the service client or controller.</summary>
        public string ClassName { get; set; }

        /// <summary>Gets or sets a value indicating whether to generate DTO classes (default: true).</summary>
        public bool GenerateDtoTypes { get; set; }

        /// <summary>Gets or sets a value indicating whether to generate interfaces for the client classes (default: false).</summary>
        public bool GenerateClientInterfaces { get; set; }

        /// <summary>Gets or sets a value indicating whether to generate client types (default: true).</summary>
        public bool GenerateClientClasses { get; set; }

        /// <summary>Gets or sets a value indicating whether to generate a class that includes all generated client types as lazy fields (default: false). If <see cref="GenerateClientInterfaces"/> is <c>true</c> then an interface for it will also be created.</summary>
        public bool GenerateCombinedClientClass { get; set; }

        /// <summary>The name of the combined client class. (default: &quot;CombinedClient&quot;).</summary>
        public string CombinedClientClassName { get; set; }

        /// <summary>Gets or sets a value indicating the generated combined client's (<see cref="GenerateCombinedClientClass"/>) constructor access modifier. Use a private constructor when you'll need custom construction logic in a partial class definition and don't want to expose the constructor to dependency-injection. (default: &quot;public&quot;).</summary>
        public string CombinedClassConstructorAccess { get; set; }

        /// <summary>Gets or sets the operation name generator.</summary>
        public IOperationNameGenerator OperationNameGenerator { get; set; }

        /// <summary>Gets or sets a value indicating whether to reorder parameters (required first, optional at the end) and generate optional parameters.</summary>
        public bool GenerateOptionalParameters { get; set; }

        /// <summary>Gets or sets the parameter name generator.</summary>
        public IParameterNameGenerator ParameterNameGenerator { get; set; }

        /// <summary>Gets or sets the globally excluded parameter names.</summary>
        public string[] ExcludedParameterNames { get; set; }

        /// <summary>Generates the name of the controller based on the provided settings.</summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <returns>The controller name.</returns>
        public string GenerateControllerName(string controllerName)
        {
            return ClassName.Replace("{controller}", ConversionUtilities.ConvertToUpperCamelCase(controllerName, false));
        }

        /// <summary>Gets or sets a value indicating whether to wrap success responses to allow full response access.</summary>
        public bool WrapResponses { get; set; }

        /// <summary>Gets or sets the list of methods where responses are wrapped ("ControllerName.MethodName", WrapResponses must be true).</summary>
        public string[] WrapResponseMethods { get; set; }

        /// <summary>Gets or sets a value indicating whether to generate the response classes (only needed when WrapResponses == true, default: true).</summary>
        public bool GenerateResponseClasses { get; set; }

        /// <summary>Gets or sets the name of the response class (supports the '{controller}' placeholder).</summary>
        public string ResponseClass { get; set; }
    }
}