//-----------------------------------------------------------------------
// <copyright file="NSwagDocumentProvider.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Threading;
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

        private int _version;
        private string _lastDocument;
        private string _lastDocumentName;

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

        public async Task<string> GenerateAsync(string documentName)
        {
            if (documentName == null)
            {
                throw new ArgumentNullException(nameof(documentName));
            }

            var apiDescriptionGroups = _apiDescriptionGroupCollectionProvider.ApiDescriptionGroups;
            var newVersion = apiDescriptionGroups.Version;
            if (_lastDocument != null &&
                string.Equals(_lastDocumentName, documentName, StringComparison.Ordinal) &&
                newVersion == Volatile.Read(ref _version))
            {
                return _lastDocument;
            }

            var document = await GenerateAsyncCore(documentName);
            _lastDocument = document.ToJson();
            _lastDocumentName = documentName;
            Volatile.Write(ref _version, newVersion);

            return _lastDocument;
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

            var document = await GenerateAsyncCore(documentName);

            var json = document.ToJson();
            await writer.WriteAsync(json);
        }

        private async Task<SwaggerDocument> GenerateAsyncCore(string documentName)
        {
            var documentSettings = _registry[documentName];
            if (documentSettings == null)
            {
                throw new InvalidOperationException(
                    $"No registered document found for document name '{documentName}'.");
            }

            documentSettings.GeneratorSettings.ApplySettings(
                _mvcJsonOptions.Value.SerializerSettings,
                _mvcOptions.Value);

            var generator = new AspNetCoreToSwaggerGenerator(
                documentSettings.GeneratorSettings,
                documentSettings.SchemaGenerator);
            var document = await generator.GenerateAsync(_apiDescriptionGroupCollectionProvider.ApiDescriptionGroups);

            documentSettings.PostProcess?.Invoke(document);

            return document;
        }
    }
}
