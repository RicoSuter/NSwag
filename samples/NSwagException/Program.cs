using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using NJsonSchema;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;

namespace NSwagException
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(x =>
                {
                    x.AddEnvironmentVariables();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).Build().RunAsync();
        }
    }

    internal class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(o => o.SerializerSettings.DateParseHandling = DateParseHandling.None);

            services.AddOpenApiDocument(s =>
            {
                s.Title = "Test";
                s.DocumentName = "neon-v1";
                s.ApiGroupNames = new[] { "myapi-v1" };
                s.SchemaType = SchemaType.OpenApi3;
            });
            services.AddRouting();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseOpenApi(options =>
            {
                options.DocumentName = "neon-v1";
                options.Path = "/swagger/v1/swagger.json";
            });

            app.UseSwaggerUi3(options =>
            {
                options.SwaggerRoutes.Add(new SwaggerUi3Route("Test", "/swagger/v1/swagger.json"));

                options.DocExpansion = "list";
                options.Path = "/swagger";
            });
        }
    }

    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "myapi-v1")]
    [Route("api/v1/test")]
    public class TestController : NeonflowControllerBase
    {
        public override Task ModifyScreenDocuments(Test_BaseClass body)
        {
            Console.WriteLine($"-->  {body.GetType().FullName}");
            return Task.CompletedTask;
        }
    }

    public abstract class NeonflowControllerBase : Microsoft.AspNetCore.Mvc.Controller
    {
        [Microsoft.AspNetCore.Mvc.HttpPost, Microsoft.AspNetCore.Mvc.Route("neonflow/modifyScreenDocuments")]
        public abstract System.Threading.Tasks.Task ModifyScreenDocuments([Microsoft.AspNetCore.Mvc.FromBody] Test_BaseClass body);
    }

[Newtonsoft.Json.JsonConverter(typeof(JsonInheritanceConverter), "discriminator")]
    [JsonInheritanceAttribute("Test_A", typeof(Test_A))]
    [JsonInheritanceAttribute("Test_B", typeof(Test_B))]
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.15.9.0 (NJsonSchema v10.6.8.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial class Test_BaseClass
    {
        [Newtonsoft.Json.JsonProperty("documentId", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Test_Property { get; set; }

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.15.9.0 (NJsonSchema v10.6.8.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial class Test_A : Test_BaseClass
    {
        [Newtonsoft.Json.JsonProperty("isUnavailable", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Test_A_Property { get; set; }

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.15.9.0 (NJsonSchema v10.6.8.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial class Test_B : Test_BaseClass
    {
        [Newtonsoft.Json.JsonProperty("isDeliveredByPost", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Test_B_Property { get; set; }

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.15.9.0 (NJsonSchema v10.6.8.0 (Newtonsoft.Json v11.0.0.0))")]
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Interface, AllowMultiple = true)]
    internal class JsonInheritanceAttribute : System.Attribute
    {
        public JsonInheritanceAttribute(string key, System.Type type)
        {
            Key = key;
            Type = type;
        }

        public string Key { get; }

        public System.Type Type { get; }
    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.15.9.0 (NJsonSchema v10.6.8.0 (Newtonsoft.Json v11.0.0.0))")]
    internal class JsonInheritanceConverter : Newtonsoft.Json.JsonConverter
    {
        internal static readonly string DefaultDiscriminatorName = "discriminator";

        private readonly string _discriminatorName;

        [System.ThreadStatic]
        private static bool _isReading;

        [System.ThreadStatic]
        private static bool _isWriting;

        public JsonInheritanceConverter()
        {
            _discriminatorName = DefaultDiscriminatorName;
        }

        public JsonInheritanceConverter(string discriminatorName)
        {
            _discriminatorName = discriminatorName;
        }

        public string DiscriminatorName { get { return _discriminatorName; } }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            try
            {
                _isWriting = true;

                var jObject = Newtonsoft.Json.Linq.JObject.FromObject(value, serializer);
                jObject.AddFirst(new Newtonsoft.Json.Linq.JProperty(_discriminatorName, GetSubtypeDiscriminator(value.GetType())));
                writer.WriteToken(jObject.CreateReader());
            }
            finally
            {
                _isWriting = false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                if (_isWriting)
                {
                    _isWriting = false;
                    return false;
                }
                return true;
            }
        }

        public override bool CanRead
        {
            get
            {
                if (_isReading)
                {
                    _isReading = false;
                    return false;
                }
                return true;
            }
        }

        public override bool CanConvert(System.Type objectType)
        {
            return true;
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, System.Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var jObject = serializer.Deserialize<Newtonsoft.Json.Linq.JObject>(reader);
            if (jObject == null)
                return null;

            var discriminatorValue = jObject.GetValue(_discriminatorName);
            var discriminator = discriminatorValue != null ? Newtonsoft.Json.Linq.Extensions.Value<string>(discriminatorValue) : null;
            var subtype = GetObjectSubtype(objectType, discriminator);

            var objectContract = serializer.ContractResolver.ResolveContract(subtype) as Newtonsoft.Json.Serialization.JsonObjectContract;
            if (objectContract == null || System.Linq.Enumerable.All(objectContract.Properties, p => p.PropertyName != _discriminatorName))
            {
                jObject.Remove(_discriminatorName);
            }

            try
            {
                _isReading = true;
                return serializer.Deserialize(jObject.CreateReader(), subtype);
            }
            finally
            {
                _isReading = false;
            }
        }

        private System.Type GetObjectSubtype(System.Type objectType, string discriminator)
        {
            foreach (var attribute in System.Reflection.CustomAttributeExtensions.GetCustomAttributes<JsonInheritanceAttribute>(System.Reflection.IntrospectionExtensions.GetTypeInfo(objectType), true))
            {
                if (attribute.Key == discriminator)
                    return attribute.Type;
            }

            return objectType;
        }

        private string GetSubtypeDiscriminator(System.Type objectType)
        {
            foreach (var attribute in System.Reflection.CustomAttributeExtensions.GetCustomAttributes<JsonInheritanceAttribute>(System.Reflection.IntrospectionExtensions.GetTypeInfo(objectType), true))
            {
                if (attribute.Type == objectType)
                    return attribute.Key;
            }

            return objectType.Name;
        }
    }
}
