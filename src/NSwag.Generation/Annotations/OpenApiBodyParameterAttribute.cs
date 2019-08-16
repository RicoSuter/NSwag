using System;
using NJsonSchema;
using NSwag.Annotations;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace NSwag.Generation
{
    public class OpenApiBodyParameterAttribute : OpenApiOperationProcessorAttribute
    {
        public OpenApiBodyParameterAttribute(string mimeType)
            : base(typeof(OpenApiBodyParameterProcessor), mimeType)
        {
        }

        internal class OpenApiBodyParameterProcessor : IOperationProcessor
        {
            private readonly string _mimeType;

            public OpenApiBodyParameterProcessor(string mimeType)
            {
                _mimeType = mimeType ?? throw new ArgumentNullException(nameof(mimeType));
            }

            public bool Process(OperationProcessorContext context)
            {
                if (context.OperationDescription.Operation.RequestBody == null)
                {
                    context.OperationDescription.Operation.RequestBody = new OpenApiRequestBody();
                }

                context.OperationDescription.Operation.RequestBody.Content[_mimeType] = new OpenApiMediaType
                {
                    Schema = new JsonSchema
                    {
                        Type = JsonObjectType.File
                    }
                };

                return true;
            }
        }
    }
}
