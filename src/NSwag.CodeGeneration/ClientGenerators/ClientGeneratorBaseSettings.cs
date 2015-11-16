//-----------------------------------------------------------------------
// <copyright file="ClientGeneratorBaseSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.ClientGenerators
{
    /// <summary>Settings for the <see cref="ClientGeneratorBase"/>.</summary>
    public class ClientGeneratorBaseSettings
    {
        /// <summary>Initializes a new instance of the <see cref="ClientGeneratorBaseSettings"/> class.</summary>
        public ClientGeneratorBaseSettings()
        {
            GenerateClientTypes = true; 
            GenerateDtoTypes = true;
        }

        /// <summary>Gets or sets a value indicating whether to generate client types (default: true).</summary>
        public bool GenerateClientTypes { get; set; }

        /// <summary>Gets or sets a value indicating whether to generate DTO classes (default: true).</summary>
        public bool GenerateDtoTypes { get; set; }
        
        /// <summary>Gets or sets a value indicating whether to generate interfaces for the client classes.</summary>
        public bool GenerateClientInterfaces { get; set; }

        /// <summary>Gets or sets the operation generation mode.</summary>
        public OperationGenerationMode OperationGenerationMode { get; set; }
    }
}