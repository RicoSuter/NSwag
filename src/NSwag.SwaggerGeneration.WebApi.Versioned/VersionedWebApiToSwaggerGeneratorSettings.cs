namespace NSwag.SwaggerGeneration.WebApi.Versioned
{
    using System.Collections.Generic;
    using Microsoft.Web.Http;
    using Processors;

    public class VersionedWebApiToSwaggerGeneratorSettings : SwaggerGeneratorSettings
    {
        public VersionedWebApiToSwaggerGeneratorSettings()
        {
            OperationProcessors.Add(new OperationParameterProcessor(this)  );
            OperationProcessors.Add(new OperationResponseProcessor(this)  );
        }

        public List<ApiVersion> ApiVersions { get; } = new List<ApiVersion>();
    }
}