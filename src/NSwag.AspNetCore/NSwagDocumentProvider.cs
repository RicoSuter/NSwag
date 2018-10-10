//-----------------------------------------------------------------------
// <copyright file="NSwagDocumentProvider.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.ApiDescription;
using Microsoft.Extensions.Options;
using NSwag.SwaggerGeneration.AspNetCore;

namespace NSwag.AspNetCore
{
    internal class NSwagDocumentProvider : IDocumentProvider
    {
        private readonly IApiDescriptionGroupCollectionProvider _apiDescriptionGroupCollectionProvider;
        private readonly IOptions<MvcOptions> _mvcOptions;
        private readonly IOptions<MvcJsonOptions> _mvcJsonOptions;
        private readonly DocumentRegistry _registry;

        public NSwagDocumentProvider(
            DocumentRegistry registry,
            IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider,
            IOptions<MvcOptions> mvcOptions,
            IOptions<MvcJsonOptions> mvcJsonOptions)
        {
            if (apiDescriptionGroupCollectionProvider == null)
            {
                throw new ArgumentNullException(nameof(apiDescriptionGroupCollectionProvider));
            }

            if (mvcOptions == null)
            {
                throw new ArgumentNullException(nameof(mvcOptions));
            }

            if (mvcJsonOptions == null)
            {
                throw new ArgumentNullException(nameof(mvcJsonOptions));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            _apiDescriptionGroupCollectionProvider = apiDescriptionGroupCollectionProvider;
            _mvcOptions = mvcOptions;
            _mvcJsonOptions = mvcJsonOptions;
            _registry = registry;
        }

        public async Task<SwaggerDocument> GenerateAsync(string documentName)
        {
            if (documentName == null)
            {
                throw new ArgumentNullException(nameof(documentName));
            }

            var documentInfo = _registry[documentName];
            if (documentInfo == null)
            {
                throw new InvalidOperationException($"No registered document found for document name '{documentName}'.");
            }

            documentInfo.Settings.ApplySettings(_mvcJsonOptions.Value.SerializerSettings, _mvcOptions.Value);

            SwaggerDocument document;
            if (documentInfo.Settings is AspNetCoreToSwaggerGeneratorSettings aspnetcore)
            {
                var generator = new AspNetCoreToSwaggerGenerator(aspnetcore, documentInfo.SchemaGenerator);
                document = await generator.GenerateAsync(_apiDescriptionGroupCollectionProvider.ApiDescriptionGroups);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported settings type '{documentInfo.GetType().FullName}");
            }

            documentInfo.PostProcess?.Invoke(document);

            return document;
        }

        // Called by the Microsoft.Extensions.ApiDescription tool
        async Task IDocumentProvider.GenerateAsync(string documentName, TextWriter writer)
        {
            if (documentName == null)
            {
                throw new ArgumentNullException(nameof(documentName));
            }

            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            var document = await GenerateAsync(documentName);

            var json = document.ToJson();
            await writer.WriteAsync(json);
        }
    }
}
