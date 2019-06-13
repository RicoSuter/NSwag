//-----------------------------------------------------------------------
// <copyright file="IDocumentProvider.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Extensions.ApiDescriptions
{
    // This service will be looked up by name from the service collection when using
    // the <c>dotnet-getdocument</c> tool from the Microsoft.Extensions.ApiDescription.Server package.
    internal interface IDocumentProvider
    {
        IEnumerable<string> GetDocumentNames();

        Task GenerateAsync(string documentName, TextWriter writer);
    }
}
