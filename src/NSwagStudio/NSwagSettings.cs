//-----------------------------------------------------------------------
// <copyright file="NSwagSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag.Commands;

namespace NSwagStudio
{
    public class NSwagSettings
    {
        public NSwagSettings()
        {
            WebApiToSwaggerCommand = new WebApiToSwaggerCommand();
            AssemblyTypeToSwaggerCommand = new AssemblyTypeToSwaggerCommand();

            SwaggerToTypeScriptCommand = new SwaggerToTypeScriptCommand();
            SwaggerToCSharpCommand = new SwaggerToCSharpCommand();
        }

        public WebApiToSwaggerCommand WebApiToSwaggerCommand { get; set; }

        public AssemblyTypeToSwaggerCommand AssemblyTypeToSwaggerCommand { get; set; }

        public SwaggerToTypeScriptCommand SwaggerToTypeScriptCommand { get; set; }

        public SwaggerToCSharpCommand SwaggerToCSharpCommand { get; set; }

    }
}
