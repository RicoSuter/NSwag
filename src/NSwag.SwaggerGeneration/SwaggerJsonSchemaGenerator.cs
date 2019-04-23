//-----------------------------------------------------------------------
// <copyright file="SwaggerJsonSchemaGenerator.cs" company="NJsonSchema">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/rsuter/NJsonSchema/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Infrastructure;

namespace NSwag.SwaggerGeneration
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
        /// <param name="type">The types.</param>
        /// <param name="typeDescription">The type desription.</param>
        /// <param name="schema">The properties</param>
        /// <param name="schemaResolver">The schema resolver.</param>
        /// <returns></returns>
        protected override async Task GenerateObjectAsync(Type type, JsonTypeDescription typeDescription, JsonSchema4 schema, JsonSchemaResolver schemaResolver)
        {
            if (_isRootType)
            {
                _isRootType = false;
                await base.GenerateObjectAsync(type, typeDescription, schema, schemaResolver);
                _isRootType = true;
            }
            else
            {
                if (!schemaResolver.HasSchema(type, false))
                {
                    _isRootType = true;
                    await GenerateAsync(type, schemaResolver);
                    _isRootType = false;
                }

                schema.Reference = schemaResolver.GetSchema(type, false);
            }
        }

        /// <summary>Generetes a schema directly or referenced for the requested schema type; also adds nullability if required.</summary>
        /// <typeparam name="TSchemaType">The resulted schema type which may reference the actual schema.</typeparam>
        /// <param name="type">The type of the schema to generate.</param>
        /// <param name="parentAttributes">The parent attributes (e.g. property or paramter attributes).</param>
        /// <param name="isNullable">Specifies whether the property, parameter or requested schema type is nullable.</param>
        /// <param name="schemaResolver">The schema resolver.</param>
        /// <param name="transformation">An action to transform the resulting schema (e.g. property or parameter) before the type of reference is determined (with $ref or allOf/oneOf).</param>
        /// <returns>The requested schema object.</returns>
        public override async Task<TSchemaType> GenerateWithReferenceAndNullabilityAsync<TSchemaType>(
            Type type, IEnumerable<Attribute> parentAttributes, bool isNullable,
            JsonSchemaResolver schemaResolver, Func<TSchemaType, JsonSchema4, Task> transformation = null)
        {
            if (type.Name == "Task`1")
                type = type.GenericTypeArguments[0];

            if (type.Name == "JsonResult`1")
                type = type.GenericTypeArguments[0];

            if (type.Name == "ActionResult`1")
                type = type.GenericTypeArguments[0];

            if (IsFileResponse(type))
            {
                if (Settings.SchemaType == SchemaType.Swagger2)
                {
                    return new TSchemaType { Type = JsonObjectType.File };
                }
                else
                {
                    return new TSchemaType { Type = JsonObjectType.String, Format = JsonFormatStrings.Binary };
                }
            }

            return await base.GenerateWithReferenceAndNullabilityAsync(type, parentAttributes, isNullable, schemaResolver, transformation);
        }

        private bool IsFileResponse(Type returnType)
        {
            return returnType.IsAssignableTo("FileResult", TypeNameStyle.Name) ||
                   returnType.Name == "IActionResult" ||
                   returnType.Name == "IHttpActionResult" ||
                   returnType.Name == "HttpResponseMessage" ||
                   returnType.IsAssignableTo("ActionResult", TypeNameStyle.Name) ||
                   returnType.InheritsFrom("HttpResponseMessage", TypeNameStyle.Name);
        }
    }
}