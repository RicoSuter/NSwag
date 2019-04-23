using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests.Web
{
    public class CustomTextInputFormatter : TextInputFormatter
    {
        // Required to use text/html or foo/bar in ConsumesAttribute, see ConsumesController and ConsumesTests

        public CustomTextInputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/html"));
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("foo/bar"));

            SupportedEncodings.Add(Encoding.UTF8);
        }

        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            return Task.FromResult(InputFormatterResult.Success(null));
        }
    }
}
