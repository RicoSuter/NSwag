//-----------------------------------------------------------------------
// <copyright file="RootTypeJsonSchemaGenerator.cs" company="NJsonSchema">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/rsuter/NJsonSchema/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using NJsonSchema;

namespace NSwag.CodeGeneration.SwaggerGenerators
{
    /// <summary>A <see cref="JsonSchemaGenerator"/> which only generate the schema for the root type. 
    /// Referenced types are added to the service's Definitions collection. </summary>
    internal class RootTypeJsonSchemaGenerator : JsonSchemaGenerator
    {
        private bool _isRootType = true;
        private readonly ISchemaDefinitionAppender _schemaDefinitionAppender;
        private readonly SwaggerService _service;

        /// <summary>Initializes a new instance of the <see cref="RootTypeJsonSchemaGenerator" /> class.</summary>
        /// <param name="service">The service.</param>
        /// <param name="schemaDefinitionAppender">The schema definition appender.</param>
        /// <param name="settings">The settings.</param>
        public RootTypeJsonSchemaGenerator(SwaggerService service, ISchemaDefinitionAppender schemaDefinitionAppender, JsonSchemaGeneratorSettings settings) : base(settings)
        {
            _service = service;
            _schemaDefinitionAppender = schemaDefinitionAppender;
        }

        /// <summary>Generates the properties for the given type and schema.</summary>
        /// <typeparam name="TSchemaType">The type of the schema type.</typeparam>
        /// <param name="type">The types.</param>
        /// <param name="schema">The properties</param>
        /// <param name="rootSchema">The root schema.</param>
        /// <param name="schemaDefinitionAppender"></param>
        /// <param name="schemaResolver">The schema resolver.</param>
        protected override void GenerateObject<TSchemaType>(Type type, TSchemaType schema, JsonSchema4 rootSchema, ISchemaDefinitionAppender schemaDefinitionAppender, ISchemaResolver schemaResolver)
        {
            if (_isRootType)
            {
                _isRootType = false;
                base.GenerateObject(type, schema, rootSchema, _schemaDefinitionAppender, schemaResolver);
            }
            else
            {
                if (!schemaResolver.HasSchema(type, false))
                {
                    var schemaGenerator = new RootTypeJsonSchemaGenerator(_service, _schemaDefinitionAppender, Settings);
                    schemaGenerator.Generate(type, rootSchema, null, _schemaDefinitionAppender, schemaResolver);
                }

                schema.SchemaReference = schemaResolver.GetSchema(type, false);
            }
        }
    }
}