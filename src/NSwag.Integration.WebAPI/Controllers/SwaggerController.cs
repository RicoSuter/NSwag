using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.Integration.WebAPI.Controllers
{
    public class SwaggerController : ApiController
    {
        private static readonly Lazy<string> _swagger = new Lazy<string>(() =>
        {
            var controllers = new[] { typeof(PersonsController) };
            var settings = new WebApiToSwaggerGeneratorSettings
            {
                DefaultUrlTemplate = "api/{controller}/{action}/{id}"
            };

            var generator = new WebApiToSwaggerGenerator(settings);
            var document = Task.Run(async () => await generator.GenerateForControllersAsync(controllers))
                .GetAwaiter().GetResult();

            return document.ToJson();
        });

        [SwaggerIgnore]
        [HttpGet, Route("swaggerdoc")]
        public HttpResponseMessage Swagger()
        {
            var response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent(_swagger.Value, Encoding.UTF8, "application/json");
            return response;
        }
    }
}