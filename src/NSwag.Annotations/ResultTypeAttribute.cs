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
    public class ResultTypeAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="ResultTypeAttribute"/> class.</summary>
        /// <param name="type">The JSON result type of the MVC or Web API action method.</param>
        public ResultTypeAttribute(Type type)
        {
            Type = type; // TODO: Check for this attribute on WebAPI methods
        }

        /// <summary>Gets or sets the JSON result type of the MVC or Web API action method.</summary>
        public Type Type { get; set; }
    }
}
