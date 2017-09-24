//-----------------------------------------------------------------------
// <copyright file="AssemblyLoaderUtilities.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using NSwag.Annotations;
using NSwag.SwaggerGeneration;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.Commands.SwaggerGenerators
{
    internal static class AssemblyLoaderUtilities
    {
        public static IEnumerable<string> GetAssemblies()
        {
            yield return "Newtonsoft.Json.dll";
            yield return "NJsonSchema.dll";
            yield return "NSwag.Core.dll";
            yield return "NSwag.Commands.dll";
            yield return "NSwag.SwaggerGeneration.dll";
            yield return "NSwag.SwaggerGeneration.WebApi.dll";
        }

        public static IEnumerable<BindingRedirect> GetBindingRedirects()
        {
#if NET451
            yield return new BindingRedirect("Newtonsoft.Json", typeof(JToken), "30ad4fe6b2a6aeed");
            yield return new BindingRedirect("Newtonsoft.Json", typeof(JToken), "30ad4fe6b2a6aeed");
            yield return new BindingRedirect("NJsonSchema", typeof(JsonSchema4), "c2f9c3bdfae56102");
            yield return new BindingRedirect("NSwag.Core", typeof(SwaggerDocument), "c2d88086e098d109");
            yield return new BindingRedirect("NSwag.SwaggerGeneration", typeof(SwaggerJsonSchemaGenerator), "c2d88086e098d109");
            yield return new BindingRedirect("NSwag.SwaggerGeneration.WebApi", typeof(WebApiToSwaggerGenerator), "c2d88086e098d109");
            yield return new BindingRedirect("NSwag.Annotations", typeof(SwaggerTagsAttribute), "c2d88086e098d109");
            yield return new BindingRedirect("System.Runtime", "4.0.0.0", "b03f5f7f11d50a3a");
            yield return new BindingRedirect("System.Private.CoreLib", "4.0.0.0", "7cec85d7bea7798e");
#else
            return new BindingRedirect[0];
#endif
        }
    }
}
