//-----------------------------------------------------------------------
// <copyright file="IXmlDocsSettings.cs" company="NJsonSchema">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NJsonSchema/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Namotion.Reflection;
using NJsonSchema.Generation;

namespace NSwag.Generation
{
    /// <summary>
    /// XML Documentation settings extensions.
    /// </summary>
    public static class XmlDocsSettingsExtensions
    {
        // TODO: Remove this class and use NJS exentions instead.

        /// <summary>
        /// Converts a settings to options.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>The options.</returns>
        public static XmlDocsOptions GetXmlDocsOptions(this IXmlDocsSettings settings)
        {
            return new XmlDocsOptions
            {
                ResolveExternalXmlDocs = settings.ResolveExternalXmlDocumentation,
                FormattingMode = settings.XmlDocumentationFormatting
            };
        }
    }
}