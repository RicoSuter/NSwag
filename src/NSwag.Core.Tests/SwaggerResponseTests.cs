using Xunit;

namespace NSwag.Core.Tests
{
    public class SwaggerResponseTests
    {
        [Theory]
        [InlineData("application/octet-stream", true)]
        [InlineData("undefined", true)]
        [InlineData("text/plain", false)]
        [InlineData("application/json", false)]
        [InlineData("application/vnd.model+json", false)]
        public void When_response_contains_produces_detect_if_binary_response(string contentType, bool expectsBinary)
        {
            // Arrange
            var response = new SwaggerResponse();
            var operation = new SwaggerOperation();
            operation.Produces = new System.Collections.Generic.List<string> { contentType };
            operation.Responses.Add("200", response);

            // Act
            var isBinary = response.IsBinary(operation);

            // Assert
            Assert.Equal(expectsBinary, isBinary);
        }
    }
}
