//-----------------------------------------------------------------------
// <copyright file="CSharpControllerOperationModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Text;
using NJsonSchema.CodeGeneration.CSharp;

namespace NSwag.CodeGeneration.CSharp.Models
{
    /// <summary>The CSharp controller operation model.</summary>
    public class CSharpControllerOperationModel : CSharpOperationModel
    {
        private readonly OpenApiOperation _operation;
        private readonly CSharpControllerGeneratorSettings _settings;
        private readonly CSharpTypeResolver _resolver;

        /// <summary>Initializes a new instance of the <see cref="CSharpControllerOperationModel" /> class.</summary>
        /// <param name="operation">The operation.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="resolver">The resolver.</param>
        public CSharpControllerOperationModel(OpenApiOperation operation, CSharpControllerGeneratorSettings settings,
            CSharpControllerGenerator generator, CSharpTypeResolver resolver)
            : base(operation, settings, generator, resolver)
        {
            _operation = operation;
            _settings = settings;
            _resolver = resolver;
        }

        /// <summary>Gets or sets the type of the result.</summary>
        public override string ResultType
        {
            get
            {
                if (_settings.UseResponseTypeAttributes)
                    return "System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult>";
                if (_settings.UseActionResultType)
                {
                    switch (SyncResultType)
                    {
                        case "void":
                        case "FileResult":
                            return "System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult>";
                        default:
                            return "System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<" + SyncResultType + ">>";
                    }
                }

                return base.ResultType;
            }
        }

        public bool HasResponseTypeAttributes => _settings.UseResponseTypeAttributes;

        public string ResponseTypeAttributes
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var response in _operation.ActualResponses)
                {
                    if (response.Value.Schema == null)
                    {
                        sb.AppendLine($"[Microsoft.AspNetCore.Mvc.ProducesResponseType({response.Key})]");
                        continue;
                    }

                    var isNullable = response.Value.IsNullable(_settings.CodeGeneratorSettings.SchemaType);
                    var responseType = _resolver.Resolve(response.Value.Schema, isNullable, "Response");
                    sb.AppendLine(
                        $"[Microsoft.AspNetCore.Mvc.ProducesResponseType({response.Key}, Type = typeof({responseType}))]");
                }

                return sb.ToString();
            }
        }
    }
}
