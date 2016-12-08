//-----------------------------------------------------------------------
// <copyright file="RequestUrlGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema;
using NSwag.CodeGeneration.CodeGenerators.Models;
using NSwag.CodeGeneration.CodeGenerators.TypeScript.Models;
using NSwag.CodeGeneration.CodeGenerators.TypeScript.Templates;

namespace NSwag.CodeGeneration.CodeGenerators.TypeScript
{
    /// <summary>Generates the code to process the response.</summary>
    public class TemplatePartGenerator
    {
        /// <summary>Renders the client class helper methods.</summary>
        /// <param name="model">The model.</param>
        /// <param name="tabCount">The tab count.</param>
        /// <returns>The helper methods.</returns>
        public static string RenderClientMethodsCode(ClientTemplateModel model, int tabCount = 0)
        {
            var tpl = new ClientMethodsTemplate(model);
            return ConversionUtilities.Tab(tpl.Render(), tabCount);
        }

        /// <summary>Renders the URL generation code.</summary>
        /// <param name="operation">The operation.</param>
        /// <param name="tabCount">The tab count.</param>
        /// <returns>Rendered request body</returns>
        public static string RenderRequestUrlCode(OperationModel operation, int tabCount = 0)
        {
            var tpl = new RequestUrlTemplate(operation);
            return ConversionUtilities.Tab(tpl.Render(), tabCount);
        }

        /// <summary>Renders the request body generation code.</summary>
        /// <param name="operation">The operation.</param>
        /// <param name="tabCount">The tab count.</param>
        /// <returns>Rendered request body</returns>
        public static string RenderRequestBodyCode(OperationModel operation, int tabCount = 0)
        {
            var tpl = new RequestBodyTemplate(operation);
            return ConversionUtilities.Tab(tpl.Render(), tabCount);
        }

        /// <summary>Renders the respone process code.</summary>
        /// <param name="operation">The operation.</param>
        /// <param name="tabCount">The tab count.</param>
        /// <returns>Rendered request body</returns>
        public static string RenderProcessResponseCode(OperationModel operation, int tabCount = 0)
        {
            var tpl = new ProcessResponseTemplate(operation);
            return ConversionUtilities.Tab(tpl.Render(), tabCount);
        }
    }
}