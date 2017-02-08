//-----------------------------------------------------------------------
// <copyright file="RequestBodyTemplate.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration.TypeScript.Models;

namespace NSwag.CodeGeneration.TypeScript.Templates
{
    internal partial class RequestBodyTemplate : ITemplate
    {
        public RequestBodyTemplate(TypeScriptOperationModel model)
        {
            Model = model;
        }

        public TypeScriptOperationModel Model { get; }

        public string Render()
        {
            return ConversionUtilities.TrimWhiteSpaces(TransformText());
        }
    }
}
