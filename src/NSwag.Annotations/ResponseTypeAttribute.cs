//-----------------------------------------------------------------------
// <copyright file="ResponseTypeAttribute.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Net;

namespace NSwag.Annotations
{
    /// <summary>Specifies the result type of a HTTP operation to correctly generate a Swagger definition.</summary>
    /// <remarks>Use <see cref="SwaggerResponseAttribute"/>, this attribute will be obsolete soon.</remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    [Obsolete("Use SwaggerResponseAttribute instead.")]
    public class ResponseTypeAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="ResponseTypeAttribute"/> class.</summary>
        /// <param name="responseType">The JSON result type of the MVC or Web API action method.</param>
        public ResponseTypeAttribute(Type responseType)
        {
            ResponseType = responseType;
        }

        /// <summary>Initializes a new instance of the <see cref="ResponseTypeAttribute"/> class.</summary>
        /// <param name="httpStatusCode">The HTTP status code for which the result type applies.</param>
        /// <param name="responseType">The JSON result type of the MVC or Web API action method.</param>
        public ResponseTypeAttribute(string httpStatusCode, Type responseType)
        {
            HttpStatusCode = httpStatusCode;
            ResponseType = responseType;
        }

        /// <summary>Initializes a new instance of the <see cref="SwaggerResponseAttribute"/> class.</summary>
        /// <param name="httpStatusCode">The HTTP status code for which the result type applies.</param>
        /// <param name="responseType">The JSON result type of the MVC or Web API action method.</param>
        public ResponseTypeAttribute(int httpStatusCode, Type responseType)
        {
            HttpStatusCode = httpStatusCode.ToString();
            ResponseType = responseType;
        }

        /// <summary>Initializes a new instance of the <see cref="SwaggerResponseAttribute"/> class.</summary>
        /// <param name="httpStatusCode">The HTTP status code for which the result type applies.</param>
        /// <param name="responseType">The JSON result type of the MVC or Web API action method.</param>
        public ResponseTypeAttribute(HttpStatusCode httpStatusCode, Type responseType)
        {
            HttpStatusCode = ((int)httpStatusCode).ToString();
            ResponseType = responseType;
        }

        /// <summary>Gets or sets the HTTP status code for which the result type applies.</summary>
        public string HttpStatusCode { get; set; }

        /// <summary>Gets or sets the JSON result type of the MVC or Web API action method.</summary>
        public Type ResponseType { get; set; }

        /// <summary>Gets or sets the description of the response.</summary>
        public string Description { get; set; }
    }
}
