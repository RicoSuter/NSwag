//-----------------------------------------------------------------------
// <copyright file="SwaggerResponseAttribute.cs" company="NSwag">
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
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SwaggerResponseAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerResponseAttribute"/> class.</summary>
        /// <param name="responseType">The JSON result type of the MVC or Web API action method.</param>
        public SwaggerResponseAttribute(Type responseType)
        {
            Type = responseType;
        }

        /// <summary>Initializes a new instance of the <see cref="SwaggerResponseAttribute"/> class.</summary>
        /// <param name="httpStatusCode">The HTTP status code for which the result type applies.</param>
        /// <param name="responseType">The JSON result type of the MVC or Web API action method.</param>
        public SwaggerResponseAttribute(string httpStatusCode, Type responseType)
        {
            StatusCode = httpStatusCode;
            Type = responseType;
        }

        /// <summary>Initializes a new instance of the <see cref="SwaggerResponseAttribute"/> class.</summary>
        /// <param name="httpStatusCode">The HTTP status code for which the result type applies.</param>
        /// <param name="responseType">The JSON result type of the MVC or Web API action method.</param>
        public SwaggerResponseAttribute(int httpStatusCode, Type responseType)
        {
            StatusCode = httpStatusCode.ToString();
            Type = responseType;
        }

        /// <summary>Initializes a new instance of the <see cref="SwaggerResponseAttribute"/> class.</summary>
        /// <param name="httpStatusCode">The HTTP status code for which the result type applies.</param>
        /// <param name="responseType">The JSON result type of the MVC or Web API action method.</param>
        public SwaggerResponseAttribute(HttpStatusCode httpStatusCode, Type responseType)
        {
            StatusCode = ((int)httpStatusCode).ToString();
            Type = responseType;
        }

        /// <summary>Gets the HTTP status code.</summary>
        public string StatusCode { get; private set; }

        /// <summary>Gets or sets the response description.</summary>
        public string Description { get; set; }

        /// <summary>Gets or sets the response type.</summary>
        public Type Type { get; set; }
    }
}