//-----------------------------------------------------------------------
// <copyright file="ReferencedJsonSchemaGenerator.cs" company="NJsonSchema">
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
    public class ReferencedJsonSchemaGenerator : JsonSchemaGenerator
    {
        private bool _isRootType = true;

        /// <summary>Initializes a new instance of the <see cref="ReferencedJsonSchemaGenerator" /> class.</summary>
        /// <param name="settings">The settings.</param>
        public ReferencedJsonSchemaGenerator(JsonSchemaGeneratorSettings settings) : base(settings)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ReferencedJsonSchemaGenerator" /> class.</summary>
        /// <param name="settings">The settings.</param>
        /// <param name="schemaResolver"></param>
        /// <param name="schemaDefinitionAppender"></param>
        public ReferencedJsonSchemaGenerator(JsonSchemaGeneratorSettings settings, ISchemaResolver schemaResolver, ISchemaDefinitionAppender schemaDefinitionAppender)
            : base(settings, schemaResolver, schemaDefinitionAppender)
        {
        }

        /// <summary>Generates the properties for the given type and schema.</summary>
        /// <typeparam name="TSchemaType">The type of the schema type.</typeparam>
        /// <param name="type">The types.</param>
        /// <param name="schema">The properties</param>
        protected override void GenerateObject<TSchemaType>(Type type, TSchemaType schema)
        {
            if (_isRootType)
            {
                _isRootType = false;
                base.GenerateObject(type, schema);
                _isRootType = true;
            }
            else
            {
                if (!SchemaResolver.HasSchema(type, false))
                {
                    _isRootType = true;
                    Generate(type);
                    _isRootType = false;
                }

                schema.SchemaReference = SchemaResolver.GetSchema(type, false);
            }
        }
    }
}