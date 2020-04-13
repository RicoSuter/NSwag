using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSwag.Commands.CodeGeneration;

namespace NSwag.Commands
{
    /// <summary>The command collection.</summary>
    public class CodeGeneratorCollection
    {
        /// <summary>Gets or sets the SwaggerToTypeScriptClientCommand.</summary>
        [JsonProperty("OpenApiToTypeScriptClient", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<OpenApiToTypeScriptClientCommand>))]
        public IEnumerable<OpenApiToTypeScriptClientCommand> OpenApiToTypeScriptClientCommands { get; set; }

        /// <summary>Gets or sets the SwaggerToCSharpClientCommand.</summary>
        [JsonProperty("OpenApiToCSharpClient", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<OpenApiToCSharpClientCommand>))]
        public IEnumerable<OpenApiToCSharpClientCommand> OpenApiToCSharpClientCommands { get; set; }

        /// <summary>Gets or sets the SwaggerToCSharpControllerCommand.</summary>
        [JsonProperty("OpenApiToCSharpController", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<OpenApiToCSharpControllerCommand>))]
        public IEnumerable<OpenApiToCSharpControllerCommand> OpenApiToCSharpControllerCommands { get; set; }

        /// <summary>Gets the items.</summary>
        [JsonIgnore]
        public IEnumerable<InputOutputCommandBase> Items => new InputOutputCommandBase[] { }
            .Concat(OpenApiToTypeScriptClientCommands ?? new OpenApiToTypeScriptClientCommand[] { })
            .Concat(OpenApiToCSharpClientCommands ?? new OpenApiToCSharpClientCommand[] { })
            .Concat(OpenApiToCSharpControllerCommands ?? new OpenApiToCSharpControllerCommand[] { })
            .Where(cmd => cmd != null);
    }


    public class SingleOrArrayConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(List<T>));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Array)
            {
                return token.ToObject<List<T>>();
            }
            return new List<T> { token.ToObject<T>() };
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}