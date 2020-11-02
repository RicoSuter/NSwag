using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace NSwag.Core.Tests.Serialization
{
    public class PathItemTests
    {
        private const string aValidPath = "/aValidPath";

        [Fact]
        public async Task PathItem_With_External_Ref_Can_Be_Serialized()
        {
            string path = GetTestDirectory() + "/Serialization/PathItemTest/PathItemWithRef.json";

            OpenApiDocument doc = await OpenApiDocument.FromFileAsync(path);

            var paths = doc.Paths;
            var pathItem = paths[aValidPath];
            var actualPathItem = pathItem.ActualPathItem;
            var getOperation = actualPathItem["get"];
            Assert.True(getOperation.ActualResponses.ContainsKey("200"));
        }

        private string GetTestDirectory()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            return Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
        }
    }
}
