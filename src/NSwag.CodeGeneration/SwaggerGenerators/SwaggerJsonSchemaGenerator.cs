//-----------------------------------------------------------------------
// <copyright file="SwaggerJsonSchemaGenerator.cs" company="NJsonSchema">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/rsuter/NJsonSchema/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using NJsonSchema;
using NJsonSchema.Generation;

namespace NSwag.CodeGeneration.SwaggerGenerators
{
    /// <summary>A <see cref="JsonSchemaGenerator"/> which only generate the schema for the root type. 
    /// Referenced types are added to the service's Definitions collection. </summary>
    public class SwaggerJsonSchemaGenerator : JsonSchemaGenerator
    {
        private bool _isRootType = true;

        /// <summary>Initializes a new instance of the <see cref="SwaggerJsonSchemaGenerator" /> class.</summary>
        /// <param name="settings">The settings.</param>
        public SwaggerJsonSchemaGenerator(JsonSchemaGeneratorSettings settings) : base(settings)
        {
        }

        /// <summary>Generates the properties for the given type and schema.</summary>
        /// <typeparam name="TSchemaType">The type of the schema type.</typeparam>
        /// <param name="type">The types.</param>
        /// <param name="schema">The properties</param>
        /// <param name="schemaResolver">The schema resolver.</param>
        protected override void GenerateObject<TSchemaType>(Type type, TSchemaType schema, JsonSchemaResolver schemaResolver)
        {
            if (_isRootType)
            {
                _isRootType = false;
                base.GenerateObject(type, schema, schemaResolver);
                _isRootType = true;
            }
            else
            {
                if (!schemaResolver.HasSchema(type, false))
                {
                    _isRootType = true;
                    Generate(type, schemaResolver);
                    _isRootType = false;
                }

                schema.SchemaReference = schemaResolver.GetSchema(type, false);
            }
        }
    }
}