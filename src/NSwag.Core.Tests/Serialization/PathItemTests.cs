using System.Threading.Tasks;
using Xunit;

namespace NSwag.Core.Tests.Serialization
{
    public class PathItemTests
    {
        [Fact]
        public async Task Foo()
        {
            //var json = @""; // TODO
            var document = await OpenApiDocument.FromFileAsync("TestFiles/pathItemTests.json");


            Assert.NotNull(document);
        }
    }
}
