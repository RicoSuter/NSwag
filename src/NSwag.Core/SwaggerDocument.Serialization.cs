//-----------------------------------------------------------------------
// <copyright file="SwaggerDocument.Serialization.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema;
using NJsonSchema.Infrastructure;

namespace NSwag
{
    public partial class SwaggerDocument
    {
        /// <summary>Creates the serializer contract resolver based on the <see cref="SchemaType"/>.</summary>
        /// <param name="schemaType">The schema type.</param>
        /// <returns>The settings.</returns>
        public static PropertyRenameAndIgnoreSerializerContractResolver CreateJsonSerializerContractResolver(SchemaType schemaType)
        {
            var resolver = JsonSchema4.CreateJsonSerializerContractResolver(schemaType);

            if (schemaType == SchemaType.OpenApi3)
            {
                resolver.IgnoreProperty(typeof(SwaggerDocument), "swagger");

                resolver.IgnoreProperty(typeof(SwaggerDocument), "definitions");
                resolver.IgnoreProperty(typeof(SwaggerDocument), "parameters");
                resolver.IgnoreProperty(typeof(SwaggerDocument), "responses");
                resolver.IgnoreProperty(typeof(SwaggerDocument), "securityDefinitions");

                resolver.IgnoreProperty(typeof(SwaggerResponse), "examples");
            }
            else if (schemaType == SchemaType.Swagger2)
            {
                resolver.IgnoreProperty(typeof(SwaggerDocument), "openapi");
                resolver.IgnoreProperty(typeof(SwaggerParameter), "title");

                resolver.IgnoreProperty(typeof(SwaggerDocument), "components");
                resolver.IgnoreProperty(typeof(SwaggerParameter), "examples");
            }

            return resolver;
        }
    }
}
