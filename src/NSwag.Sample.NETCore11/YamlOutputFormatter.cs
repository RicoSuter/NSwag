using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace NSwag.Sample.NETCore11
{
    public class YamlOutputFormatter : OutputFormatter
    {
        public YamlOutputFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/yaml"));
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context) => Task.CompletedTask;
    }
}
