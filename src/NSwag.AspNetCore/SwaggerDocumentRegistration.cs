//-----------------------------------------------------------------------
// <copyright file="RegisteredSwaggerDocument.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag.SwaggerGeneration;

namespace NSwag.AspNetCore
{
    internal class SwaggerDocumentRegistration
    {
        public SwaggerDocumentRegistration(string documentName, ISwaggerGenerator generator)
        {
            DocumentName = documentName;
            Generator = generator;
        }

        public string DocumentName { get; }

        public ISwaggerGenerator Generator { get; }
    }
}
