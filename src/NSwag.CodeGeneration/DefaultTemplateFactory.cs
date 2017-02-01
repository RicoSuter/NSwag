//-----------------------------------------------------------------------
// <copyright file="DefaultTemplateFactory.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Reflection;
using NJsonSchema.CodeGeneration;

namespace NSwag.CodeGeneration
{
    /// <summary>The default template factory which loads templates from embedded resources.</summary>
    public class DefaultTemplateFactory : NJsonSchema.CodeGeneration.DefaultTemplateFactory
    {
        /// <summary>Creates a template for the given language, template name and template model.</summary>
        /// <param name="package">The package name (i.e. language).</param>
        /// <param name="template">The template name.</param>
        /// <param name="model">The template model.</param>
        /// <returns>The template.</returns>
        /// <remarks>Supports NJsonSchema and NSwag embedded templates.</remarks>
        public override ITemplate CreateTemplate(string package, string template, object model)
        {
            var typeName = "NSwag.CodeGeneration." + package + ".Templates." + template + "Template";
            var type = Type.GetType(typeName);
            if (type == null)
                type = Assembly.Load(new AssemblyName("NSwag.CodeGeneration." + package))?.GetType(typeName);

            if (type != null)
                return (ITemplate)Activator.CreateInstance(type, model);
            else
                return base.CreateTemplate(package, template, model);
        }
    }
}
