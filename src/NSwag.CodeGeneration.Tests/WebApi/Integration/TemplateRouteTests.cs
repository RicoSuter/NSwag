using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.CodeGeneration.Tests.WebApi.Integration
{
    [TestClass]
    public class TemplateRouteTests
    {
        public class ValuesController : ApiController
        {
            public class DeviceList
            {
                public Guid Id { get; set; }
            }
            public class Device : DeviceList
            {
                public string Name { get; set; }
            }

            [Swashbuckle.Swagger.Annotations.SwaggerOperation("GetAllDevices")]
            [Swashbuckle.Swagger.Annotations.SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<DeviceList>))]
            public IEnumerable<DeviceList> Get(string SortCriteria = "", int page = 1, int pagesize = 10)
            {
                return null;
            }

            [Swashbuckle.Swagger.Annotations.SwaggerOperation("GetDeviceById")]
            [Swashbuckle.Swagger.Annotations.SwaggerResponse(HttpStatusCode.OK, Type = typeof(Device))]
            [Swashbuckle.Swagger.Annotations.SwaggerResponse(HttpStatusCode.NotFound)]
            public Device Get(string id)
            {
                try
                {
                    return new Device();
                }
                catch (System.Exception err)
                {
                    throw;
                }
            }

            [Swashbuckle.Swagger.Annotations.SwaggerOperation("CreateDevice")]
            [Swashbuckle.Swagger.Annotations.SwaggerResponse(HttpStatusCode.Created)]
            [Swashbuckle.Swagger.Annotations.SwaggerResponse(HttpStatusCode.OK, Type = typeof(Device))]
            public async Task<Device> Post([FromBody] Device value)
            {
                try
                {
                    return new Device();
                }
                catch (System.Exception err)
                {
                    throw;
                }
            }

            [Swashbuckle.Swagger.Annotations.SwaggerOperation("UpdateDevice")]
            [Swashbuckle.Swagger.Annotations.SwaggerResponse(HttpStatusCode.OK, Type = typeof(Device))]
            [Swashbuckle.Swagger.Annotations.SwaggerResponse(HttpStatusCode.NotFound)]
            [Swashbuckle.Swagger.Annotations.SwaggerResponse(HttpStatusCode.Conflict, Type = typeof(HttpResponseException))]

            public async Task<Device> Put(string id, [FromBody] Device value)
            {
                try
                {
                    return new Device();
                }
                catch (System.Exception err)
                {
                    throw;
                }
            }

            [Swashbuckle.Swagger.Annotations.SwaggerOperation("DeleteDevice")]
            [Swashbuckle.Swagger.Annotations.SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
            [Swashbuckle.Swagger.Annotations.SwaggerResponse(HttpStatusCode.NotFound)]
            public async Task<bool> Delete(string id)
            {
                try
                {
                    var deleted = true;

                    return deleted;
                }
                catch (System.Exception err)
                {
                    throw;
                }
            }
        }

        [TestMethod]
        public async Task When_optional_id_is_used_then_generation_works()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());

            //// Act
            var document = await generator.GenerateForControllerAsync<ValuesController>();
            var json = document.ToJson();

            //// Assert
            Assert.IsTrue(json != null);
        }
    }
}