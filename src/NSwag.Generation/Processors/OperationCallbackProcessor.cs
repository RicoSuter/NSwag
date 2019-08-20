//-----------------------------------------------------------------------
// <copyright file="OperationTagsProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Namotion.Reflection;
using NSwag.Generation.Processors.Contexts;
using NJsonSchema;

namespace NSwag.Generation.Processors
{
    /// <summary>Processes the SwaggerCallbackAttribute on the operation method.</summary>
    public class OperationCallbackProcessor : IOperationProcessor
    {
        /// <summary>
        /// Processes the specified method information.
        /// </summary>
        /// <param name="context">The processor context.</param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        public bool Process(OperationProcessorContext context)
        {
            var operation = context.OperationDescription.Operation;
            if (operation.Callbacks == null)
            {
                operation.Callbacks = new Dictionary<string, OpenApiCallback>();
            }

            foreach (var CallbackAttribute in
                    from CallbackAttribute
                    in context.MethodInfo.GetCustomAttributes()
                        .GetAssignableToTypeName("OpenApiCallbackAttribute", TypeNameStyle.Name)
                    select (dynamic)CallbackAttribute)
            {
                string url = CallbackAttribute.CallbackUrl;
                string key = CallbackAttribute.Name ?? context.OperationDescription.Operation.OperationId + "_Callback";
                string method = CallbackAttribute.Method ?? OpenApiOperationMethod.Post;
                string mimeType = CallbackAttribute.MimeType;
                Type[] types = CallbackAttribute.Types;

                var payloadSchemas = types.Select(t =>
                    context.SchemaGenerator.GenerateWithReferenceAndNullability<NJsonSchema.JsonSchema>(
                        t.ToContextualType(),
                        context.SchemaResolver
                    )).ToArray();

                var binarySchemas = payloadSchemas.Where(x => x != null && x.IsBinary).ToArray();
                var jsonSchemas = payloadSchemas.Where(x => x != null && !x.IsBinary).ToArray();

                OpenApiRequestBody requestBody;

                if (payloadSchemas.Length == 0)
                {
                    requestBody = null;
                }
                else
                {
                    requestBody = new OpenApiRequestBody { IsRequired = true };

                    if (!String.IsNullOrWhiteSpace(mimeType))
                    {
                        requestBody.Content[mimeType] = CreateMediaType(payloadSchemas);
                    }
                    else
                    {
                        requestBody.Content["application/octet-stream"] = CreateMediaType(binarySchemas);
                        requestBody.Content["application/json"] = CreateMediaType(jsonSchemas);
                    }
                };


                operation.Callbacks[key] =
                    new OpenApiCallback
                        {
                            {
                                url,
                                new OpenApiPathItem
                                {
                                    {
                                        method,
                                        new OpenApiOperation
                                        {
                                            RequestBody = requestBody
                                        }
                                    }
                                }
                            }
                        };


            }

            return true;
        }


        private OpenApiMediaType CreateMediaType(JsonSchema[] schemas)
        {
            if (schemas.Length == 0)
            {
                return null;
            }
            else if (schemas.Length == 1)
            {
                return new OpenApiMediaType { Schema = schemas[0] };
            }
            else
            {
                var parentSchema = new JsonSchema();
                foreach (var schema in schemas)
                {
                    parentSchema.OneOf.Add(schema);
                }
                return new OpenApiMediaType { Schema = parentSchema }; ;
            }
        }
    }
}