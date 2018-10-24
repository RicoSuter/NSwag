//-----------------------------------------------------------------------
// <copyright file="SwaggerDocumentSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using NSwag.SwaggerGeneration;
using NSwag.SwaggerGeneration.AspNetCore;

namespace NSwag.AspNetCore
{
    /// <summary>
    /// Swagger and Open API document settings.
    /// </summary>
    public class SwaggerDocumentSettings
    {
        private SwaggerJsonSchemaGenerator _schemaGenerator;

        /// <summary>
        /// Gets or sets the document name. The document name is used as a logical identifier inside NSwag.
        /// </summary>
        /// <value>Defaults to "v1".</value>
        public string DocumentName { get; set; } = "v1";

        /// <summary>
        /// Gets the <see cref="AspNetCoreToSwaggerGeneratorSettings"/> for this document.
        /// </summary>
        public AspNetCoreToSwaggerGeneratorSettings GeneratorSettings { get; } = new AspNetCoreToSwaggerGeneratorSettings();

        /// <summary>
        /// Gets or sets the <see cref="SwaggerJsonSchemaGenerator"/>.
        /// </summary>
        /// <value>
        /// If left <see langword="null"/>, gets an instance initialized using <see cref="GeneratorSettings"/>.
        /// </value>
        public SwaggerJsonSchemaGenerator SchemaGenerator
        {
            get
            {
                if (_schemaGenerator == null)
                {
                    _schemaGenerator = new SwaggerJsonSchemaGenerator(GeneratorSettings);
                }

                return _schemaGenerator;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _schemaGenerator = value;
            }
        }
    }
}
