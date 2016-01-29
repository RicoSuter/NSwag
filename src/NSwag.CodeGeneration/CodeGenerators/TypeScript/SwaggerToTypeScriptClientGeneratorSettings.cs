//-----------------------------------------------------------------------
// <copyright file="SwaggerToTypeScriptClientGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.CodeGenerators.TypeScript
{
    /// <summary>Settings for the <see cref="SwaggerToTypeScriptClientGenerator"/>.</summary>
    public class SwaggerToTypeScriptClientGeneratorSettings : CodeGeneratorBaseSettings
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerToTypeScriptClientGeneratorSettings"/> class.</summary>
        public SwaggerToTypeScriptClientGeneratorSettings()
        {
            ModuleName = "";
            ClassName = "{controller}Client";
            Template = TypeScriptTemplate.JQueryCallbacks;
            PromiseType = PromiseType.Promise;
        }

        /// <summary>Gets or sets the class name of the service client.</summary>
        public string ClassName { get; set; }

        /// <summary>Gets or sets the output template.</summary>
        public TypeScriptTemplate Template { get; set; }

        /// <summary>Gets or sets the promise type.</summary>
        public PromiseType PromiseType { get; set; }

        /// <summary>Gets or sets the TypeScript module name (default: '', no module).</summary>
        public string ModuleName { get; set; }
    }
}