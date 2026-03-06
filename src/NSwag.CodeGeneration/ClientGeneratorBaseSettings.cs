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
            SuppressClientClassesOutput = false;
            SuppressClientInterfacesOutput = false;
            GenerateDtoTypes = true;

            OperationNameGenerator = new MultipleClientsFromOperationIdOperationNameGenerator();
            IncludedOperationIds = [];
            ExcludedOperationIds = [];
            ParameterNameGenerator = new DefaultParameterNameGenerator();

            GenerateResponseClasses = true;
            ResponseClass = "SwaggerResponse";

            WrapResponseMethods = [];
            ExcludedParameterNames = [];
        }

        /// <summary>Gets the code generator settings.</summary>
        public abstract CodeGeneratorSettingsBase CodeGeneratorSettings { get; }

        /// <summary>Gets or sets the class name of the service client or controller.</summary>
        public string ClassName { get; set; }

        /// <summary>Gets or sets a value indicating whether to generate DTO classes (default: true).</summary>
        public bool GenerateDtoTypes { get; set; }

        /// <summary>Gets or sets a value indicating whether to generate interfaces for the client classes (default: false).</summary>
        public bool GenerateClientInterfaces { get; set; }

        /// <summary>Gets or sets a value indicating whether to generate the output of interfaces for the client classes (default: false).</summary>
        public bool SuppressClientInterfacesOutput { get; set; }

        /// <summary>Gets or sets a value indicating whether to generate client types (default: true).</summary>
        public bool GenerateClientClasses { get; set; }

        /// <summary>Gets or sets a value indicating whether to generate the output of client types (default: false).</summary>
        public bool SuppressClientClassesOutput { get; set; }

        /// <summary>Gets or sets the operation name generator.</summary>
        public IOperationNameGenerator OperationNameGenerator { get; set; }
        
        /// <summary>Gets or sets the only operations that should be included in the generated client.</summary>
        public string[] IncludedOperationIds { get; set; }
        
        /// <summary>Gets or sets the operations that should be excluded from the generated client.</summary>
        public string[] ExcludedOperationIds { get; set; }

        /// <summary>Gets or sets the value indicating if deprecated endpoints shall be rendered</summary>
        public bool ExcludeDeprecated { get; set; }

        /// <summary>Gets or sets a value indicating whether to reorder parameters (required first, optional at the end) and generate optional parameters.</summary>
        public bool GenerateOptionalParameters { get; set; }

        /// <summary>Gets or sets the parameter name generator.</summary>
        public IParameterNameGenerator ParameterNameGenerator { get; set; }

        /// <summary>Gets or sets the globally excluded parameter names.</summary>
        public string[] ExcludedParameterNames { get; set; }

        /// <summary>Generates the name of the controller based on the provided settings.</summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <returns>The controller name.</returns>
        public virtual string GenerateControllerName(string controllerName)
        {
            controllerName = controllerName.Replace('.', '_').Replace('-', '_');
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