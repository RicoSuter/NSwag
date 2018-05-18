//-----------------------------------------------------------------------
// <copyright file="TypeScriptClientTemplateModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using NJsonSchema.CodeGeneration.TypeScript;

namespace NSwag.CodeGeneration.TypeScript.Models
{
    /// <summary>The TypeScript client template model.</summary>
    public class TypeScriptClientTemplateModel
    {
        private readonly TypeScriptExtensionCode _extensionCode;
        private readonly SwaggerToTypeScriptClientGeneratorSettings _settings;
        private readonly SwaggerDocument _document;

        /// <summary>Initializes a new instance of the <see cref="TypeScriptClientTemplateModel" /> class.</summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="controllerClassName">Name of the controller.</param>
        /// <param name="operations">The operations.</param>
        /// <param name="extensionCode">The extension code.</param>
        /// <param name="document">The Swagger document.</param>
        /// <param name="settings">The settings.</param>
        public TypeScriptClientTemplateModel(
            string controllerName,
            string controllerClassName,
            IEnumerable<TypeScriptOperationModel> operations,
            TypeScriptExtensionCode extensionCode,
            SwaggerDocument document,
            SwaggerToTypeScriptClientGeneratorSettings settings)
        {
            _extensionCode = extensionCode;
            _settings = settings;
            _document = document;

            Class = controllerClassName;
            Operations = operations;

            BaseClass = _settings.ClientBaseClass?.Replace("{controller}", controllerName);
        }

        /// <summary>Gets the class name.</summary>
        public string Class { get; }

        /// <summary>Gets a value indicating whether the client class has a base class.</summary>
        public bool HasBaseClass => !string.IsNullOrEmpty(BaseClass);

        /// <summary>Gets the client base class.</summary>
        public string BaseClass { get; }

        /// <summary>Gets or sets a value indicating whether to use the getBaseUrl(defaultUrl: string) from the base class.</summary>
        public bool UseGetBaseUrlMethod => _settings.UseGetBaseUrlMethod;

        /// <summary>Gets the configuration class name.</summary>
        public string ConfigurationClass => _settings.ConfigurationClass;

        /// <summary>Gets a value indicating whether the client class has a base class.</summary>
        public bool HasConfigurationClass => HasBaseClass && !string.IsNullOrEmpty(ConfigurationClass);

        /// <summary>Gets or sets a value indicating whether to call 'transformOptions' on the base class or extension class.</summary>
        public bool UseTransformOptionsMethod => _settings.UseTransformOptionsMethod;

        /// <summary>Gets or sets a value indicating whether to call 'transformResult' on the base class or extension class.</summary>
        public bool UseTransformResultMethod => _settings.UseTransformResultMethod;

        /// <summary>Gets a value indicating whether to generate optional parameters.</summary>
        public bool GenerateOptionalParameters => _settings.GenerateOptionalParameters;

        /// <summary>Gets a value indicating whether the client is extended with an extension class.</summary>
        public bool HasExtensionCode => _settings.TypeScriptGeneratorSettings.ExtendedClasses?.Any(c => c == Class) == true;

        /// <summary>Gets the extension body code.</summary>
        public string ExtensionCode => _extensionCode.GetExtensionClassBody(Class);

        /// <summary>Gets or sets a value indicating whether the extension code has a constructor and no constructor has to be generated.</summary>
        public bool HasExtendedConstructor => HasExtensionCode && ExtensionCode.Contains("constructor(");

        /// <summary>Gets a value indicating whether the client has operations.</summary>
        public bool HasOperations => Operations.Any();

        /// <summary>Gets the operations.</summary>
        public IEnumerable<TypeScriptOperationModel> Operations { get; }

        /// <summary>Gets a value indicating whether the client uses KnockoutJS.</summary>
        public bool UsesKnockout => _settings.TypeScriptGeneratorSettings.TypeStyle == TypeScriptTypeStyle.KnockoutClass;

        /// <summary>Gets the service base URL.</summary>
        public string BaseUrl => _document.BaseUrl;

        /// <summary>Gets a value indicating whether to generate client interfaces.</summary>
        public bool GenerateClientInterfaces => _settings.GenerateClientInterfaces;

        /// <summary>Gets the promise type.</summary>
        public string PromiseType => _settings.PromiseType == TypeScript.PromiseType.Promise ? "Promise" : "Q.Promise";

        /// <summary>Gets the promise constructor code.</summary>
        public string PromiseConstructor => _settings.PromiseType == TypeScript.PromiseType.Promise ? "new Promise" : "Q.Promise";

        /// <summary>Gets or sets a value indicating whether to use Aurelia HTTP injection.</summary>
        public bool UseAureliaHttpInjection => _settings.Template == TypeScriptTemplate.Aurelia;

        /// <summary>Gets a value indicating whether the target TypeScript version supports strict null checks.</summary>
        public bool SupportsStrictNullChecks => _settings.TypeScriptGeneratorSettings.TypeScriptVersion >= 2.0m;

        /// <summary>Gets or sets a value indicating whether DTO exceptions are wrapped in a SwaggerException instance.</summary>
        public bool WrapDtoExceptions => _settings.WrapDtoExceptions;

        /// <summary>Gets or sets the null value used for query parameters which are null.</summary>
        public string QueryNullValue => _settings.QueryNullValue;

        /// <summary>Gets a value indicating whether to render for AngularJS.</summary>
        public bool IsAngularJS => _settings.Template == TypeScriptTemplate.AngularJS;

        /// <summary>Gets a value indicating whether to render for Angular2.</summary>
        public bool IsAngular => _settings.Template == TypeScriptTemplate.Angular;

        /// <summary>Gets a value indicating whether to render for JQuery.</summary>
        public bool IsJQuery => _settings.Template == TypeScriptTemplate.JQueryCallbacks ||
                                _settings.Template == TypeScriptTemplate.JQueryPromises;

        /// <summary>Gets a value indicating whether to render for Fetch or Aurelia</summary>
        public bool IsFetchOrAurelia => _settings.Template == TypeScriptTemplate.Fetch ||
                                        _settings.Template == TypeScriptTemplate.Aurelia;

        public string ObservableMergeMapMethod => UseRxJs5 ? "flatMap" : "observableMergeMap";

        public string ObservableCatchMethod => UseRxJs5 ? "catch" : "observableCatch";

        public string ObservableOfMethod => UseRxJs5 ? "Observable.of" : "observableOf";

        public string ObservableFromMethod => UseRxJs5 ? "Observable.fromPromise" : "observableFrom";

        public string ObservableThrowMethod => UseRxJs5 ? "Observable.throw" : "observableThrow";

        // Angular only

        /// <summary>Gets or sets the token name for injecting the API base URL string (used in the Angular2 template).</summary>
        public string BaseUrlTokenName => _settings.BaseUrlTokenName;

        /// <summary>Gets a value indicating whether to use HttpClient with the Angular template.</summary>
        public bool UseAngularHttpClient => _settings.HttpClass == HttpClass.HttpClient;

        /// <summary>Gets the HTTP client class name.</summary>
        public string AngularHttpClass => UseAngularHttpClient ? "HttpClient" : "Http";

        /// <summary>Gets a value indicating to use RxJS5 with Angular template..</summary>
        public bool UseRxJs5 => !_settings.UseRxJs6;

        /// <summary>Gets a value indicating to use RxJS6 with Angular template..</summary>
        public bool UseRxJs6 => _settings.UseRxJs6;

        /// <summary>Gets a value indicating to use new Angular 6 Singleton Provider with Angular template..</summary>
        public bool UseSingletonProvider => _settings.UseSingletonProvider;
    }
}