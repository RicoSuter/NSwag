//-----------------------------------------------------------------------
// <copyright file="SwaggerToTypeScriptClientGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.TypeScript;

namespace NSwag.CodeGeneration.TypeScript
{
    /// <summary>Settings for the <see cref="SwaggerToTypeScriptClientGenerator"/>.</summary>
    public class SwaggerToTypeScriptClientGeneratorSettings : ClientGeneratorBaseSettings
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerToTypeScriptClientGeneratorSettings"/> class.</summary>
        public SwaggerToTypeScriptClientGeneratorSettings()
        {
            ClassName = "{controller}Client";
            Template = TypeScriptTemplate.Fetch;
            PromiseType = PromiseType.Promise;
            TypeScriptGeneratorSettings = new TypeScriptGeneratorSettings
            {
                NullHandling = NullHandling.Swagger,
                MarkOptionalProperties = true,
                TemplateFactory = new DefaultTemplateFactory(), 
                TypeNameGenerator = new TypeScriptTypeNameGenerator()
            };
            BaseUrlTokenName = "API_BASE_URL";
            ImportRequiredTypes = true;
        }

        /// <summary>Gets or sets the TypeScript generator settings.</summary>
        public TypeScriptGeneratorSettings TypeScriptGeneratorSettings { get; }

        /// <summary>Gets the code generator settings.</summary>
        public override CodeGeneratorSettingsBase CodeGeneratorSettings => TypeScriptGeneratorSettings;

        /// <summary>Gets or sets the output template.</summary>
        public TypeScriptTemplate Template { get; set; }

        /// <summary>Gets or sets the promise type.</summary>
        public PromiseType PromiseType { get; set; }

        /// <summary>Gets or sets a value indicating whether DTO exceptions are wrapped in a SwaggerException instance (default: false).</summary>
        public bool WrapDtoExceptions { get; set; }

        /// <summary>Gets or sets the client base class.</summary>
        public string ClientBaseClass { get; set; }

        /// <summary>Gets or sets the full name of the configuration class (<see cref="ClientBaseClass"/> must be set).</summary>
        public string ConfigurationClass { get; set; }

        /// <summary>Gets or sets a value indicating whether to call 'transformOptions' on the base class or extension class.</summary>
        public bool UseTransformOptionsMethod { get; set; }

        /// <summary>Gets or sets a value indicating whether to call 'transformResult' on the base class or extension class.</summary>
        public bool UseTransformResultMethod { get; set; }

        /// <summary>Gets or sets the token name for injecting the API base URL string (used in the Angular2 template, default: '').</summary>
        public string BaseUrlTokenName { get; set; }

        /// <summary>Gets or sets the list of methods with a protected access modifier ("classname.methodname").</summary>
        public string[] ProtectedMethods { get; set; }

        /// <summary>Gets or sets a value indicating whether required types should be imported (default: true).</summary>
        public bool ImportRequiredTypes { get; set; } = true;

        /// <summary>Gets or sets a value indicating whether to use the 'getBaseUrl(defaultUrl: string)' from the base class (default: false).</summary>
        public bool UseGetBaseUrlMethod { get; set; }

        internal ITemplate CreateTemplate(object model)
        {
            if (Template == TypeScriptTemplate.Aurelia)
                return CodeGeneratorSettings.TemplateFactory.CreateTemplate("TypeScript", TypeScriptTemplate.Fetch + "Client", model);

            return CodeGeneratorSettings.TemplateFactory.CreateTemplate("TypeScript", Template + "Client", model);
        }
    }
}