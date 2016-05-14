//-----------------------------------------------------------------------
// <copyright file="ControllerGeneratorBaseSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.CodeGenerators
{
    /// <summary>Settings for the <see cref="ClientGeneratorBase"/>.</summary>
    public class ControllerGeneratorBaseSettings
    {
        /// <summary>Initializes a new instance of the <see cref="ClientGeneratorBaseSettings"/> class.</summary>
        public ControllerGeneratorBaseSettings()
        {
            GenerateDtoTypes = true;
        }

        /// <summary>Gets or sets a value indicating whether to generate DTO classes (default: true).</summary>
        public bool GenerateDtoTypes { get; set; }
    }
}