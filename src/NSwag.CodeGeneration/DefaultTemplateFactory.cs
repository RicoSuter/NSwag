﻿//-----------------------------------------------------------------------
// <copyright file="DefaultTemplateFactory.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Reflection;
using NJsonSchema.CodeGeneration;
using System.IO;

namespace NSwag.CodeGeneration
{
    /// <summary>The default template factory which loads templates from embedded resources.</summary>
    public class DefaultTemplateFactory : NJsonSchema.CodeGeneration.DefaultTemplateFactory
    {
        /// <summary>Initializes a new instance of the <see cref="DefaultTemplateFactory" /> class.</summary>
        /// <param name="settings">The settings.</param>
        /// <param name="assemblies">The assemblies.</param>
        public DefaultTemplateFactory(CodeGeneratorSettingsBase settings, Assembly[] assemblies)
            : base(settings, assemblies)
        {
        }

        /// <summary>Gets the current toolchain version.</summary>
        /// <returns>The toolchain version.</returns>
        protected override string GetToolchainVersion()
        {
            if (!string.Equals(Environment.GetEnvironmentVariable("NSWAG_NOVERSION"), "true", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(Environment.GetEnvironmentVariable("NSWAG_NOVERSION"), "1", StringComparison.OrdinalIgnoreCase))
            {
                return $"{OpenApiDocument.ToolchainVersion} (NJsonSchema v{base.GetToolchainVersion()})";
            }

            return "";
        }

        /// <summary>Tries to load an embedded Liquid template.</summary>
        /// <param name="language">The language.</param>
        /// <param name="template">The template name.</param>
        /// <returns>The template.</returns>
        protected override string GetEmbeddedLiquidTemplate(string language, string template)
        {
            template = template.TrimEnd('!');
            var assembly = GetLiquidAssembly("NSwag.CodeGeneration." + language);
            var resourceName = $"NSwag.CodeGeneration.{language}.Templates.{template}.liquid";

            var resource = assembly.GetManifestResourceStream(resourceName);
            if (resource != null)
            {
                using (var reader = new StreamReader(resource))
                {
                    return reader.ReadToEnd();
                }
            }

            return base.GetEmbeddedLiquidTemplate(language, template);
        }
    }
}
