//-----------------------------------------------------------------------
// <copyright file="HttpUtilities.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag
{
    public static class HttpUtilities
    {
        public static bool IsSuccessStatusCode(string statusCode)
        {
            return statusCode.Length == 3 && statusCode.StartsWith("2");
        }
    }
}
