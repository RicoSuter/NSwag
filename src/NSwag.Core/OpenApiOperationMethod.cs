//-----------------------------------------------------------------------
// <copyright file="SwaggerOperationMethod.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag
{
    /// <summary>Enumeration of the available HTTP methods. </summary>
    public static class OpenApiOperationMethod
    {
        /// <summary>An undefined method.</summary>
        public const string Undefined = "undefined";

        /// <summary>The HTTP GET method. </summary>
        public const string Get = "get";

        /// <summary>The HTTP POST method. </summary>
        public const string Post = "post";

        /// <summary>The HTTP PUT method. </summary>
        public const string Put = "put";

        /// <summary>The HTTP DELETE method. </summary>
        public const string Delete = "delete";

        /// <summary>The HTTP OPTIONS method. </summary>
        public const string Options = "options";

        /// <summary>The HTTP HEAD method. </summary>
        public const string Head = "head";

        /// <summary>The HTTP PATCH method. </summary>
        public const string Patch = "patch";

        /// <summary>The HTTP TRACE method (OpenAPI only). </summary>
        public const string Trace = "trace";
    }
}