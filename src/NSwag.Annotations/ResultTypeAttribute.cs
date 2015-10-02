//-----------------------------------------------------------------------
// <copyright file="ResultTypeAttribute.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace NSwag.Annotations
{
    /// <summary>Specifies the result type of a web service method to correctly generate a Swagger definition.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ResultTypeAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="ResultTypeAttribute"/> class.</summary>
        /// <param name="type">The JSON result type of the MVC or Web API action method.</param>
        public ResultTypeAttribute(Type type)
        {
            HttpStatusCode = "200";
            Type = type;
        }

        /// <summary>Initializes a new instance of the <see cref="ResultTypeAttribute"/> class.</summary>
        /// <param name="httpStatusCode">The HTTP status code for which the result type applies.</param>
        /// <param name="type">The JSON result type of the MVC or Web API action method.</param>
        public ResultTypeAttribute(string httpStatusCode, Type type)
        {
            HttpStatusCode = httpStatusCode; 
            Type = type;
        }

        /// <summary>Gets or sets the HTTP status code for which the result type applies.</summary>
        public string HttpStatusCode { get; set; }

        /// <summary>Gets or sets the JSON result type of the MVC or Web API action method.</summary>
        public Type Type { get; set; }

    }
}
