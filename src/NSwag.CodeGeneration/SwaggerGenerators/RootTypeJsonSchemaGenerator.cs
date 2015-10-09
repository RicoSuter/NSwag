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
        private readonly SwaggerService _service;

        /// <summary>Initializes a new instance of the <see cref="RootTypeJsonSchemaGenerator" /> class.</summary>
        /// <param name="service">The service.</param>
        public RootTypeJsonSchemaGenerator(SwaggerService service)
        {
            _service = service;
        }

        /// <summary>Generates the properties for the given type and schema.</summary>
        /// <typeparam name="TSchemaType">The type of the schema type.</typeparam>
        /// <param name="type">The types.</param>
        /// <param name="schema">The properties</param>
        /// <param name="schemaResolver">The schema resolver.</param>
        protected override void GenerateObject<TSchemaType>(Type type, TSchemaType schema, ISchemaResolver schemaResolver)
        {
            if (_isRootType)
            {
                _isRootType = false;
                base.GenerateObject(type, schema, schemaResolver);
            }
            else
            {
                if (!schemaResolver.HasSchema(type))
                {
                    var schemaGenerator = new RootTypeJsonSchemaGenerator(_service);
                    schemaGenerator.Generate<JsonSchema4>(type, schemaResolver);
                }

                schema.SchemaReference = schemaResolver.GetSchema(type);
            }
        }
    }
}