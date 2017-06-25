using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using NSwag.Annotations;
using NSwag.Integration.WebAPI.Models;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.Integration.WebAPI.Controllers
{
    public class FilterOptions
    {
        [JsonProperty("currentStates")]
        public string[] CurrentStates { get; set; }
    }

    public class GeoController : ApiController
    {
        public void FromBodyTest([FromBody] GeoPoint location)
        {

        }

        public void FromUriTest([FromUri] GeoPoint location)
        {

        }

        [HttpPost]
        public void AddPolygon(GeoPoint[] points)
        {

        }

        public void Filter([FromUri] FilterOptions filter)
        {

        }

        [HttpPost]
        public string[] Reverse([FromUri] string[] values)
        {
            return values.Reverse().ToArray();
        }

        [ResponseType("204", typeof(void))]
        public object Refresh()
        {
            throw new NotSupportedException();
        }

        public bool UploadFile(HttpPostedFileBase file)
        {
            return file.InputStream.ReadByte() == 1 && file.InputStream.ReadByte() == 2;
        }

        public void UploadFiles(IEnumerable<HttpPostedFileBase> files)
        {

        }

        [HttpPost]
        [ResponseType("204", typeof(void))]
        [ResponseType("450", typeof(Exception), Description = "A custom error occured.")]
        public void SaveItems(GenericRequest<Address, Person> request)
        {
            throw new ArgumentException("Test");
        }

        public HttpResponseMessage GetUploadedFile(int id, bool @override = false)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(new byte[] { 1, 2, 3 })
            };
        }

        [HttpPost]
        public double? PostDouble([FromUri]double? value = null)
        {
            // This allows us to test whether the client correctly converted the parameter to a string before adding it to the uri.
            if (!value.HasValue)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return value;
        }

        #region Swagger generator

        private static readonly Lazy<string> _swagger = new Lazy<string>(() =>
        {
            var settings = new WebApiToSwaggerGeneratorSettings
            {
                DefaultUrlTemplate = "api/{controller}/{action}/{id}"
            };

            var generator = new WebApiToSwaggerGenerator(settings);
            var document = Task.Run(async () => await generator.GenerateForControllerAsync<GeoController>())
                .GetAwaiter().GetResult();

            return document.ToJson();
        });

        [SwaggerIgnore]
        [HttpGet, Route("api/Geo/Swagger")]
        public HttpResponseMessage Swagger()
        {
            var response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent(_swagger.Value, Encoding.UTF8, "application/json");
            return response;
        }

        #endregion
    }
}