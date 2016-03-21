//-----------------------------------------------------------------------
// <copyright file="CodeGeneratorBaseSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using NSwag.CodeGeneration.CodeGenerators.OperationNameGenerators;

namespace NSwag.CodeGeneration.CodeGenerators
{
    /// <summary>Settings for the <see cref="ClientGeneratorBase"/>.</summary>
    public class CodeGeneratorBaseSettings
    {
        /// <summary>Initializes a new instance of the <see cref="CodeGeneratorBaseSettings"/> class.</summary>
        public CodeGeneratorBaseSettings()
        {
            GenerateClientClasses = true; 
            GenerateDtoTypes = true;
        }

        /// <summary>Gets or sets the operation generation mode.</summary>
        public OperationGenerationMode OperationGenerationMode { get; set; }

        /// <summary>Gets or sets a value indicating whether to generate DTO classes (default: true).</summary>
        public bool GenerateDtoTypes { get; set; }
        
        /// <summary>Gets or sets a value indicating whether to generate interfaces for the client classes.</summary>
        public bool GenerateClientInterfaces { get; set; }

        /// <summary>Gets or sets a value indicating whether to generate client types (default: true).</summary>
        public bool GenerateClientClasses { get; set; }

        /// <exception cref="NotSupportedException" accessor="get">The OperationGenerationMode is not supported.</exception>
        internal IOperationNameGenerator OperationNameGenerator
        {
            get
            {
                if (OperationGenerationMode == OperationGenerationMode.MultipleClientsFromOperationId)
                    return new MultipleClientsFromOperationIdOperationNameGenerator();

                if (OperationGenerationMode == OperationGenerationMode.MultipleClientsFromPathSegments)
                    return new MultipleClientsFromPathSegmentsOperationNameGenerator();

                if (OperationGenerationMode == OperationGenerationMode.SingleClientFromOperationId)
                    return new SingleClientFromOperationIdOperationNameGenerator();

                throw new NotSupportedException("The OperationGenerationMode " + OperationGenerationMode + " is not supported.");
            }
        }
    }
}