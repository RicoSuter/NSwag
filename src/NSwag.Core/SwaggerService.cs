//-----------------------------------------------------------------------
// <copyright file="SwaggerService.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NJsonSchema;
using NSwag.Collections;

namespace NSwag
{
    /// <summary>Describes a JSON web service.</summary>
    public class SwaggerService
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerService"/> class.</summary>
        public SwaggerService()
        {
            Swagger = "2.0";
            Info = new SwaggerInfo();
            Schemes = new List<SwaggerSchema>();
            Responses = new Dictionary<string, SwaggerResponse>();
            SecurityDefinitions = new Dictionary<string, SwaggerSecurityScheme>();

            Info = new SwaggerInfo
            {
                Version = string.Empty, 
                Title = string.Empty
            };

            Definitions = new ObservableDictionary<string, JsonSchema4>();
            Definitions.CollectionChanged += (sender, args) =>
            {
                foreach (var pair in Definitions.Where(p => string.IsNullOrEmpty(p.Value.TypeNameRaw)))
                    pair.Value.TypeNameRaw = pair.Key;
            };

            Paths = new ObservableDictionary<string, SwaggerOperations>();
            Paths.CollectionChanged += (sender, args) =>
            {
                foreach (var path in Paths.Values)
                    path.Parent = this;
            };
        }

        /// <summary>Gets the NSwag toolchain version.</summary>
        public static string ToolchainVersion => typeof(SwaggerService).GetTypeInfo().Assembly.GetName().Version.ToString();

        /// <summary>Gets or sets the Swagger specification version being used.</summary>
        [JsonProperty(PropertyName = "swagger", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Swagger { get; set; }

        /// <summary>Gets or sets the metadata about the API.</summary>
        [JsonProperty(PropertyName = "info", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public SwaggerInfo Info { get; set; }

        /// <summary>Gets or sets the host (name or ip) serving the API.</summary>
        [JsonProperty(PropertyName = "host", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Host { get; set; }

        /// <summary>Gets or sets the base path on which the API is served, which is relative to the <see cref="Host"/>.</summary>
        [JsonProperty(PropertyName = "basePath", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string BasePath { get; set; }

        /// <summary>Gets or sets the schemes.</summary>
        [JsonProperty(PropertyName = "schemes", DefaultValueHandling = DefaultValueHandling.Ignore, ItemConverterType = typeof(StringEnumConverter))]
        public List<SwaggerSchema> Schemes { get; set; }

        /// <summary>Gets or sets a list of MIME types the operation can consume.</summary>
        [JsonProperty(PropertyName = "consumes", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Consumes { get; set; }

        /// <summary>Gets or sets a list of MIME types the operation can produce.</summary>
        [JsonProperty(PropertyName = "produces", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Produces { get; set; }

        /// <summary>Gets or sets the operations.</summary>
        [JsonProperty(PropertyName = "paths", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public ObservableDictionary<string, SwaggerOperations> Paths { get; private set; }

        /// <summary>Gets or sets the types.</summary>
        [JsonProperty(PropertyName = "definitions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ObservableDictionary<string, JsonSchema4> Definitions { get; private set; }

        /// <summary>Gets or sets the parameters which can be used for all operations.</summary>
        [JsonProperty(PropertyName = "parameters", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<SwaggerParameter> Parameters { get; set; }

        /// <summary>Gets or sets the responses which can be used for all operations.</summary>
        [JsonProperty(PropertyName = "responses", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, SwaggerResponse> Responses { get; private set; }

        /// <summary>Gets or sets the security definitions.</summary>
        [JsonProperty(PropertyName = "securityDefinitions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, SwaggerSecurityScheme> SecurityDefinitions { get; private set; }

        /// <summary>Gets or sets a security description.</summary>
        [JsonProperty(PropertyName = "security", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<SwaggerSecurityRequirement> Security { get; set; }

        /// <summary>Gets or sets the description.</summary>
        [JsonProperty(PropertyName = "tags", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<SwaggerTag> Tags { get; set; }

        /// <summary>Gets the base URL of the web service.</summary>
        [JsonIgnore]
        public string BaseUrl
        {
            get
            {
                if (string.IsNullOrEmpty(Host))
                    return "";

                if (Schemes.Any())
                    return (Schemes.First().ToString().ToLowerInvariant() + "://" + Host + (!string.IsNullOrEmpty(BasePath) ? "/" + BasePath.Trim('/') : string.Empty)).Trim('/');

                return ("http://" + Host + (!string.IsNullOrEmpty(BasePath) ? "/" + BasePath.Trim('/') : string.Empty)).Trim('/');
            }
        }

        /// <summary>Gets or sets the external documentation.</summary>
        [JsonProperty(PropertyName = "externalDocs", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public SwaggerExternalDocumentation ExternalDocumentation { get; set; }

        /// <summary>Converts the description object to JSON.</summary>
        /// <returns>The JSON string.</returns>
        public string ToJson()
        {
            return ToJson(null);
        }

        /// <summary>Converts the description object to JSON.</summary>
        /// <param name="typeNameGenerator">The type name generator.</param>
        /// <returns>The JSON string.</returns>
        public string ToJson(ITypeNameGenerator typeNameGenerator)
        {
            var settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                Formatting = Formatting.Indented
            };

            GenerateOperationIds();

            JsonSchemaReferenceUtilities.UpdateSchemaReferencePaths(this, new SwaggerServiceSchemaDefinitionAppender(this, typeNameGenerator));
            var data = JsonConvert.SerializeObject(this, settings);
            JsonSchemaReferenceUtilities.UpdateSchemaReferences(this);

            return JsonSchemaReferenceUtilities.ConvertPropertyReferences(data);
        }

        /// <summary>Creates a description object from a JSON string.</summary>
        /// <param name="data">The JSON data.</param>
        /// <returns>The <see cref="SwaggerService"/>.</returns>
        public static SwaggerService FromJson(string data)
        {
            data = JsonSchemaReferenceUtilities.ConvertJsonReferences(data);
            var service = JsonConvert.DeserializeObject<SwaggerService>(data, new JsonSerializerSettings
            {
                ConstructorHandling = ConstructorHandling.Default,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });
            JsonSchemaReferenceUtilities.UpdateSchemaReferences(service);
            return service;
        }

        /// <summary>Creates a description object from a JSON string.</summary>
        /// <param name="url">The URL.</param>
        /// <returns>The <see cref="SwaggerService"/>.</returns>
        public static SwaggerService FromUrl(string url)
        {
            dynamic client = Activator.CreateInstance(Type.GetType("System.Net.WebClient, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", true));
            using (client)
            {
                var data = client.DownloadString(url);
                return FromJson(data);
            }
        }

        /// <summary>Gets the operations.</summary>
        [JsonIgnore]
        public IEnumerable<SwaggerOperationDescription> Operations
        {
            get
            {
                return Paths.SelectMany(p => p.Value.Select(o => new SwaggerOperationDescription
                {
                    Path = p.Key,
                    Method = o.Key,
                    Operation = o.Value
                }));
            }
        }

        /// <summary>Generates the missing or non-unique operation IDs.</summary>
        public void GenerateOperationIds()
        {
            // TODO: Check this method for correctness

            var groups = Operations.GroupBy(o => o.Operation.OperationId);
            foreach (var group in groups)
            {
                var operations = group.ToList();

                if (group.Count() > 1)
                {
                    var arrayResponseOperation = operations.FirstOrDefault(
                        a => a.Operation.Responses.Any(r => HttpUtilities.IsSuccessStatusCode(r.Key) && r.Value.Schema != null && r.Value.Schema.Type == JsonObjectType.Array));

                    if (arrayResponseOperation != null)
                    {
                        var name = GetOperationName(arrayResponseOperation) + "All";
                        if (Operations.All(o => o.Operation.OperationId != name))
                        {
                            arrayResponseOperation.Operation.OperationId = name;
                            operations.Remove(arrayResponseOperation);
                        }
                    }

                    var index = 0;
                    foreach (var operation in operations)
                    {
                        var name = GetOperationName(operation);
                        if (operation.Operation.OperationId != name)
                        {
                            string fullName;
                            do
                            {
                                fullName = index > 0 ? name + index : name;
                                index++;
                            } while (Operations.Any(o => o.Operation.OperationId == fullName));

                            operation.Operation.OperationId = fullName;
                        }
                    }
                }
                else if (string.IsNullOrEmpty(group.Key))
                    operations.First().Operation.OperationId = GetOperationName(operations.First());
            }
        }

        private string GetOperationName(SwaggerOperationDescription operation)
        {
            var pathSegments = operation.Path.Trim('/').Split('/');
            var lastPathSegment = pathSegments.LastOrDefault(s => !s.Contains("{"));

            // TODO: Also check routes
            return !string.IsNullOrEmpty(operation.Operation.OperationId)
                ? operation.Operation.OperationId
                : lastPathSegment;
        }
    }
}