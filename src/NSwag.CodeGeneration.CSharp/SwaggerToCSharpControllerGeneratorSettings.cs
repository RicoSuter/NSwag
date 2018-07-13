//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpControllerGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag.CodeGeneration.CSharp.Models;

namespace NSwag.CodeGeneration.CSharp
{
    // TODO: Rename to SwaggerToCSharpControllerGeneratorSettings?

    /// <summary>Settings for the <see cref="SwaggerToCSharpControllerGenerator"/>.</summary>
    public class SwaggerToCSharpControllerGeneratorSettings : SwaggerToCSharpGeneratorSettings
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerToCSharpControllerGeneratorSettings"/> class.</summary>
        public SwaggerToCSharpControllerGeneratorSettings()
        {
            ClassName = "{controller}";
            CSharpGeneratorSettings.ArrayType = "System.Collections.Generic.List";
            ControllerStyle = CSharpControllerStyle.Partial;
            UseCancellationToken = false;
            AspNetNamespace = "System.Web.Http";
            RouteNamingStrategy = CSharpControllerRouteNamingStrategy.None;
        }

        /// <summary>Returns the route name for a controller method.</summary>
        /// <param name="operation">Swagger operation</param>
        /// <returns>Route name.</returns>
        public string GetRouteName(SwaggerOperation operation)
        {
            if (RouteNamingStrategy == CSharpControllerRouteNamingStrategy.OperationId)
            {
                return operation.OperationId;
            }

            return null;
        }

        /// <summary>Gets or sets the full name of the base class.</summary>
        public string ControllerBaseClass { get; set; }

        /// <summary>Gets or sets the controller generation style (partial, abstract; default: partial).</summary>
        public CSharpControllerStyle ControllerStyle { get; set; }

        /// <summary>Gets or sets a value indicating whether to allow adding cancellation token </summary>
        public bool UseCancellationToken { get; set; }

        /// <summary>Gets or sets the ASP.NET namespace (default: 'System.Web.Http').</summary>
        public string AspNetNamespace { get; set; }

        /// <summary>Gets or sets the strategy for naming routes (default: CSharpRouteNamingStrategy.None).</summary>
        public CSharpControllerRouteNamingStrategy RouteNamingStrategy { get; set; }
    }
}