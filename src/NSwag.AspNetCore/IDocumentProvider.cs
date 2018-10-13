//-----------------------------------------------------------------------
// <copyright file="IDocumentProvider.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Extensions.ApiDescription
{
    // This service will be looked up by name from the service collection when using
    // the Microsoft.Extensions.ApiDescription tool
    internal interface IDocumentProvider
    {
        Task GenerateAsync(string documentName, TextWriter writer);
    }
}
