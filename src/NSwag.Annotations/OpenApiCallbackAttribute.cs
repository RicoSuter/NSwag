//-----------------------------------------------------------------------
// <copyright file="SwaggerTagAttribute.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace NSwag.Annotations
{
    /// <summary>Indicates a callback to be added to the Swagger definition.</summary>
    /// <remarks>Requires the SwaggerCallbackOperationProcessor to be used in the Swagger definition generation.</remarks>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class OpenApiCallbackAttribute : Attribute
    {

        /// <summary>Initializes a new instance of the <see cref="OpenApiCallbackAttribute"/> class.</summary>
        /// <param name="name"></param>
        /// <param name="callbackUrl"></param>
        /// <param name="method"></param>
        /// <param name="types"></param>
        public OpenApiCallbackAttribute(string callbackUrl,
                                        string name = null, 
                                        string method = "post",
                                        params Type[] types) : base()
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Invalid callback name", nameof(name));
            }

            this.Name = name;
            this.Types = types;
            this.CallbackUrl = callbackUrl;
            this.Method = method;
        }

        /// <summary>
        /// The name of the callback
        /// </summary>
        public string Name { get; }


        /// <summary>
        /// The URL of the endpoint that will receive the callback
        /// </summary>
        public string CallbackUrl { get; }

        /// <summary>
        /// The HTTP method that will be used to call the endpoint
        /// </summary>
        public string Method { get; }

        /// <summary>
        /// The possible object types returned by the callback
        /// </summary>
        public Type[] Types { get; }
    }
}
