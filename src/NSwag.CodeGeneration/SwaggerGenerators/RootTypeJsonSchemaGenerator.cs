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

        /// <summary>Initializes a new instance of the <see cref="RootTypeJsonSchemaGenerator"/> class.</summary>
        /// <param name="service">The service.</param>
        public RootTypeJsonSchemaGenerator(SwaggerService service)
        {
            _service = service;
        }

        /// <summary>Generates the properties for the given type and schema.</summary>
        /// <typeparam name="TSchemaType"></typeparam>
        /// <param name="type">The types.</param>
        /// <param name="schema">The properties</param>
        protected override void GenerateObjectProperties<TSchemaType>(Type type, TSchemaType schema)
        {
            if (_isRootType)
            {
                _isRootType = false;
                base.GenerateObjectProperties(type, schema);
            }
            else
            {
                if (!_service.Definitions.ContainsKey(type.Name))
                {
                    var schemaGenerator = new RootTypeJsonSchemaGenerator(_service);
                    _service.Definitions[type.Name] = schemaGenerator.Generate<JsonSchema4>(type);
                }

                schema.SchemaReference = _service.Definitions[type.Name];
            }
        }
    }
}