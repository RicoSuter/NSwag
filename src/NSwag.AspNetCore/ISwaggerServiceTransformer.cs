//-----------------------------------------------------------------------
// <copyright file="ISwaggerServiceTransformer.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.AspNetCore
{
    /// <summary>Transforms a Swagger service after its generation.</summary>
    public interface ISwaggerServiceTransformer
    {
        /// <summary>Transforms the Swagger service.</summary>
        void Transform(SwaggerService service);
    }
}