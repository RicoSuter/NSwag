//-----------------------------------------------------------------------
// <copyright file="OpenApiHeaders.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag.Collections;

namespace NSwag
{
    /// <summary>A collection of headers.</summary>
    public sealed class OpenApiHeaders : ObservableDictionary<string, OpenApiHeader>
    {
    }
}