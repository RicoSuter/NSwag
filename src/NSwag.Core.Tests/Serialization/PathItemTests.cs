using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using NJsonSchema;
using NJsonSchema.Generation;
using Xunit;

namespace NSwag.Core.Tests.Serialization
{
    public class PathItemTests
    {
        private const string aValidPath = "/aValidPath";

        [Fact]
        public async Task PathItem_With_External_Ref_Can_Be_Serialized()
        {
            string refPath = GetTestDirectory() + "/Serialization/PathItemTest/refs/PathItem.json";
            JsonSchema refPathItem = await JsonSchema.FromFileAsync(refPath);

            string path = GetTestDirectory() + "/Serialization/PathItemTest/PathItemWithRef.json";

            OpenApiDocument result = await OpenApiDocument.FromFileAsync(path,
					  schema4 =>
					  {
						  JsonSchemaResolver schemaResolver = new JsonSchemaResolver(
							 schema4,
							 new JsonSchemaGeneratorSettings());

						  var resolver = new JsonReferenceResolver(schemaResolver);
						  // someone referencing "definitions.schema.json? Use this content instead
						  resolver.AddDocumentReference(refPath, refPathItem);

						  return resolver;
					  });

            var resultPaths = result.Paths;
            var resultPathItem = resultPaths[aValidPath];
            var actualPathItem = resultPathItem.ActualPathItem;
            var resultOperation = actualPathItem["get"];
            Assert.True(resultOperation.ActualResponses.ContainsKey("200"));
        }

        private string GetTestDirectory()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            return Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
        }
    }
}
