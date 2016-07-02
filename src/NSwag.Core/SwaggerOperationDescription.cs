//-----------------------------------------------------------------------
// <copyright file="SwaggerOperationDescription.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag
{
    /// <summary>Flattened information about an operation.</summary>
    public class SwaggerOperationDescription
    {
        /// <summary>Gets or sets the relative URL path.</summary>
        public string Path { get; set; }

        /// <summary>Gets or sets the HTTP method.</summary>
        public SwaggerOperationMethod Method { get;  set; }

        /// <summary>Gets or sets the operation.</summary>
        public SwaggerOperation Operation { get; set; }
    }
}