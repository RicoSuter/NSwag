//-----------------------------------------------------------------------
// <copyright file="HttpUtilities.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag
{
    /// <summary>Contains HTTP utilities.</summary>
    public static class HttpUtilities
    {
        /// <summary>Checks whether the given HTTP status code indicates success.</summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <returns>true if success.</returns>
        public static bool IsSuccessStatusCode(string statusCode)
        {
            return statusCode.Length == 3 && statusCode.StartsWith("2");
        }
    }
}
