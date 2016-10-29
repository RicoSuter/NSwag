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

namespace NSwag.CodeGeneration.CodeGenerators.TypeScript
{
    /// <summary>Settings for the <see cref="SwaggerToTypeScriptClientGenerator"/>.</summary>
    public class SwaggerToTypeScriptClientGeneratorSettings : ClientGeneratorBaseSettings
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerToTypeScriptClientGeneratorSettings"/> class.</summary>
        public SwaggerToTypeScriptClientGeneratorSettings()
        {
            ClassName = "{controller}Client";
            Template = TypeScriptTemplate.JQueryCallbacks;
            PromiseType = PromiseType.Promise;
            TypeScriptGeneratorSettings = new TypeScriptGeneratorSettings
            {
                NullHandling = NullHandling.Swagger
            };
        }

        /// <summary>Gets or sets the TypeScript generator settings.</summary>
        public TypeScriptGeneratorSettings TypeScriptGeneratorSettings { get; set; }

        /// <summary>Gets the code generator settings.</summary>
        public override CodeGeneratorSettingsBase CodeGeneratorSettings => TypeScriptGeneratorSettings;

        /// <summary>Gets or sets the output template.</summary>
        public TypeScriptTemplate Template { get; set; }

        /// <summary>Gets or sets the promise type.</summary>
        public PromiseType PromiseType { get; set; }

        internal ITemplate CreateTemplate(object model)
        {
            if (Template == TypeScriptTemplate.Aurelia)
                return CodeGeneratorSettings.TemplateFactory.CreateTemplate("TypeScript", TypeScriptTemplate.Fetch + "Client", model);

            return CodeGeneratorSettings.TemplateFactory.CreateTemplate("TypeScript", Template + "Client", model);
        }
    }
}