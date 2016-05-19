//-----------------------------------------------------------------------
// <copyright file="SwaggerParameterKind.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NSwag
{
    /// <summary>Defines the collectionFormat of a parameter.</summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SwaggerParameterCollectionFormat
    {
        /// <summary>An undefined format.</summary>
        [EnumMember(Value = "undefined")]
        Undefined,

        /// <summary>Comma separated values "foo, bar".</summary>
        [EnumMember(Value = "csv")]
        Csv,

        /// <summary>Space separated values "foo bar".</summary>
        [EnumMember(Value = "ssv")]
        Ssv,

        /// <summary>Tab separated values "foo\tbar".</summary>
        [EnumMember(Value = "tsv")]
        Tsv,

        /// <summary>Pipe separated values "foo|bar".</summary>
        [EnumMember(Value = "pipes")]
        Pipes,

        /// <summary>Corresponds to multiple parameter instances instead of multiple values for a single instance "foo=bar&amp;foo=baz".</summary>
        [EnumMember(Value = "multi")]
        Multi
    }
}