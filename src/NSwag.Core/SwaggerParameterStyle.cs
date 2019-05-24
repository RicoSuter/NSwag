using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NSwag
{
    /// <summary>Enumeration of the parameter kinds. </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OpenApiParameterStyle
    {
        /// <summary>An undefined kind.</summary>
        [EnumMember(Value = "undefined")]
        Undefined,

        /// <summary>Comma-separated values. For Path parameters.
        /// Corresponds to the {param_name} URI template
        /// </summary>
        [EnumMember(Value = "simple")]
        Simple,

        /// <summary>Dot-prefixed values, also known as label expansion. For Path parameters.
        /// Corresponds to the {.param_name} URI template
        /// </summary>
        [EnumMember(Value = "label")]
        Label,

        /// <summary>Semicolon-prefixed values, also known as path-style expansion.
        /// For path parameters. Corresponds to the {;param_name} URI template
        /// </summary>
        [EnumMember(Value = "matrix")]
        Matrix,

        /// <summary>Ampersand-separated values. Also known as form-style query expansion.
        /// Corresponds to the {?param_name} URI template.
        /// </summary>
        [EnumMember(Value = "form")]
        Form,

        /// <summary>Space-separated array values. Same as collectionFormat: ssv in OpenAPI 2.0.
        /// Has effect only for non-exploded arrays (explode: false), that is, the space
        /// separates the array values if the array is a single parameter, as in arr=a b c.
        /// </summary>
        [EnumMember(Value = "spaceDelimited")]
        SpaceDelimeted,

        /// <summary>Pipeline-separated array values. Same as collectionFormat: pipes in
        /// OpenAPI 2.0. Has effect only for non-exploded arrays (explode: false),
        /// that is, the pipe separates the array values if the array is a single
        /// parameter, as in arr=a|b|c.</summary>
        [EnumMember(Value = "pipeDelimited")]
        PipeDelimited,

        /// <summary>Simple way of rendering nested objects using form parameters (applies to objects only)</summary>
        [EnumMember(Value = "deepObject")]
        DeepObject,
    }
}