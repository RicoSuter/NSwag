//-----------------------------------------------------------------------
// <copyright file="SwaggerDocumentYaml.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using YamlDotNet.Serialization;

namespace NSwag
{
    /// <summary>Extension methods to load and save <see cref="SwaggerDocument"/> from/to YAML.</summary>
    public static class SwaggerDocumentYaml
    {
        /// <summary>Creates a Swagger specification from a YAML string.</summary>
        /// <param name="data">The JSON data.</param>
        /// <param name="documentPath">The document path (URL or file path) for resolving relative document references.</param>
        /// <returns>The <see cref="SwaggerDocument"/>.</returns>
        public static async Task<SwaggerDocument> FromYamlAsync(string data, string documentPath = null)
        {
            var deserializer = new DeserializerBuilder().Build();
            var yamlObject = deserializer.Deserialize(new StringReader(data));

            var serializer = new SerializerBuilder()
                .JsonCompatible()
                .Build();

            var json = serializer.Serialize(yamlObject);
            return await SwaggerDocument.FromJsonAsync(json, documentPath).ConfigureAwait(false);
        }

        /// <summary>Converts the Swagger specification to YAML.</summary>
        /// <returns>The YAML string.</returns>
        public static string ToYaml(this SwaggerDocument document)
        {
            var json = document.ToJson();
            var expConverter = new ExpandoObjectConverter();
            dynamic deserializedObject = JsonConvert.DeserializeObject<ExpandoObject>(json, expConverter);

            var serializer = new Serializer();
            return serializer.Serialize(deserializedObject);
        }
    }
}
