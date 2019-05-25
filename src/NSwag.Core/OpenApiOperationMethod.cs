//-----------------------------------------------------------------------
// <copyright file="SwaggerOperationMethod.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag
{
    /// <summary>Enumeration of the available HTTP methods. </summary>
    public static class OpenApiOperationMethod
    {
        /// <summary>An undefined method.</summary>
        public static string Undefined { get; } = "undefined";

        /// <summary>The HTTP GET method. </summary>
        public static string Get { get; } = "get";

        /// <summary>The HTTP POST method. </summary>
        public static string Post { get; } = "post";

        /// <summary>The HTTP PUT method. </summary>
        public static string Put { get; } = "put";

        /// <summary>The HTTP DELETE method. </summary>
        public static string Delete { get; } = "delete";

        /// <summary>The HTTP OPTIONS method. </summary>
        public static string Options { get; } = "options";

        /// <summary>The HTTP HEAD method. </summary>
        public static string Head { get; } = "head";

        /// <summary>The HTTP PATCH method. </summary>
        public static string Patch { get; } = "patch";

        /// <summary>The HTTP TRACE method (OpenAPI only). </summary>
        public static string Trace { get; } = "trace";
    }
}