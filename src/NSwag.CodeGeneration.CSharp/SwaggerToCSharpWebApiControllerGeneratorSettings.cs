//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpClientGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag.CodeGeneration.CSharp.Models;

namespace NSwag.CodeGeneration.CSharp
{
    // TODO: Rename to SwaggerToCSharpControllerGeneratorSettings?

    /// <summary>Settings for the <see cref="SwaggerToCSharpWebApiControllerGenerator"/>.</summary>
    public class SwaggerToCSharpWebApiControllerGeneratorSettings : SwaggerToCSharpGeneratorSettings
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerToCSharpWebApiControllerGeneratorSettings"/> class.</summary>
        public SwaggerToCSharpWebApiControllerGeneratorSettings()
        {
            ClassName = "{controller}";
            CSharpGeneratorSettings.ArrayType = "System.Collections.Generic.List";
            ControllerStyle = CSharpControllerStyle.Partial;
        }

        /// <summary>Gets or sets the full name of the base class.</summary>
        public string ControllerBaseClass { get; set; }

        /// <summary>Gets or sets the controller generation style (partial, abstract; default: partial).</summary>
        public CSharpControllerStyle ControllerStyle { get; set; }
    }
}