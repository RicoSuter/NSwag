//-----------------------------------------------------------------------
// <copyright file="CSharpControllerOperationModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema.CodeGeneration.CSharp;

namespace NSwag.CodeGeneration.CSharp.Models
{
    /// <summary>The CSharp controller operation model.</summary>
    public class CSharpControllerOperationModel : CSharpOperationModel
    {
        private readonly SwaggerToCSharpControllerGeneratorSettings _settings;

        /// <summary>Initializes a new instance of the <see cref="CSharpControllerOperationModel" /> class.</summary>
        /// <param name="operation">The operation.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="resolver">The resolver.</param>
        public CSharpControllerOperationModel(SwaggerOperation operation, SwaggerToCSharpControllerGeneratorSettings settings, 
            SwaggerToCSharpControllerGenerator generator, CSharpTypeResolver resolver) 
            : base(operation, settings, generator, resolver)
        {
            _settings = settings;
        }

        /// <summary>Gets or sets the type of the result.</summary>
        public override string ResultType
        {
            get
            {
                if (_settings.UseActionResultType)
                {
                    return SyncResultType == "void"
                        ? "System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult>"
                        : "System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<" + SyncResultType + ">>";
                }

                return base.ResultType;
            }
        }
    }
}
