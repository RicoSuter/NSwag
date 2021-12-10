//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpClientGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.CSharp
{
    /// <summary>Settings for the <see cref="CSharpClientGenerator"/>.</summary>
    public class CSharpClientGeneratorSettings : CSharpGeneratorBaseSettings
    {
        /// <summary>Initializes a new instance of the <see cref="CSharpClientGeneratorSettings"/> class.</summary>
        public CSharpClientGeneratorSettings()
        {
            ClassName = "{controller}Client";

            GenerateExceptionClasses = true;
            ExceptionClass = "ApiException";
            ClientClassAccessModifier = "public";
            UseBaseUrl = true;
            HttpClientType = "System.Net.Http.HttpClient";
            WrapDtoExceptions = true;
            DisposeHttpClient = true;
            ParameterDateTimeFormat = "s";
            ParameterDateFormat = "yyyy-MM-dd";
            GenerateUpdateJsonSerializerSettingsMethod = true;
            UseRequestAndResponseSerializationSettings = false;
            QueryNullValue = "";
            GenerateBaseUrlProperty = true;
            ExposeJsonSerializerSettings = false;
            InjectHttpClient = true;
            ProtectedMethods = new string[0];
        }

        /// <summary>Gets or sets the full name of the base class.</summary>
        public string ClientBaseClass { get; set; }

        /// <summary>Gets or sets the full name of the base interface.</summary>
        public string ClientBaseInterface { get; set; }

        /// <summary>Gets or sets the full name of the configuration class (<see cref="ClientBaseClass"/> must be set).</summary>
        public string ConfigurationClass { get; set; }

        /// <summary>Gets or sets a value indicating whether to generate exception classes (default: true).</summary>
        public bool GenerateExceptionClasses { get; set; }

        /// <summary>Gets or sets the name of the exception class (supports the '{controller}' placeholder, default 'ApiException').</summary>
        public string ExceptionClass { get; set; }

        /// <summary>Gets or sets a value indicating whether an HttpClient instance is injected into the client (default: true).</summary>
        public bool InjectHttpClient { get; set; }

        /// <summary>Gets or sets a value indicating whether to dispose the HttpClient (injected HttpClient is never disposed, default: true).</summary>
        public bool DisposeHttpClient { get; set; }

        /// <summary>Gets or sets the list of methods with a protected access modifier ("classname.methodname").</summary>
        public string[] ProtectedMethods { get; set; }

        /// <summary>Gets or sets a value indicating whether to call CreateHttpClientAsync on the base class to create a new HttpClient instance (cannot be used when the HttpClient is injected).</summary>
        public bool UseHttpClientCreationMethod { get; set; }

        /// <summary>Gets or sets a value indicating whether to call CreateHttpRequestMessageAsync on the base class to create a new HttpRequestMethod.</summary>
        public bool UseHttpRequestMessageCreationMethod { get; set; }

        /// <summary>Gets or sets a value indicating whether DTO exceptions are wrapped in a SwaggerException instance (default: true).</summary>
        public bool WrapDtoExceptions { get; set; }

        /// <summary>Gets or sets the client class access modifier (default: public).</summary>
        public string ClientClassAccessModifier { get; set; }

        /// <summary>Gets or sets a value indicating whether to use and expose the base URL (default: true).</summary>
        public bool UseBaseUrl { get; set; }

        /// <summary>Gets or sets a value indicating whether to generate the BaseUrl property, must be defined on the base class otherwise (default: true).</summary>
        public bool GenerateBaseUrlProperty { get; set; }

        /// <summary>Gets or sets a value indicating whether to generate synchronous methods (not recommended, default: false).</summary>
        public bool GenerateSyncMethods { get; set; }

        /// <summary>
        /// Gets or sets the HttpClient type which will be used in the generation of the client code. By default the System.Net.Http.HttpClient
        /// will be used, but this can be overridden. Just keep in mind that the type you specify has the same default HttpClient method signatures.
        /// </summary>
        public string HttpClientType { get; set; }

        /// <summary>Gets or sets the format for DateTime type method parameters (default: "s").</summary>
        public string ParameterDateTimeFormat { get; set; }

        /// <summary>Gets or sets the format for Date type method parameters (default: "yyyy-MM-dd").</summary>
        public string ParameterDateFormat { get; set; }

        /// <summary>Gets or sets a value indicating whether to generate the UpdateJsonSerializerSettings method (must be implemented in the base class otherwise, default: true).</summary>
        public bool GenerateUpdateJsonSerializerSettingsMethod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to create PrepareRequest and ProcessResponse as async methods, or as partial synchronous methods.
        /// If value is set to true, PrepareRequestAsync and ProcessResponseAsync methods must be implemented as part of the client base class (if it has one) or as part of the partial client class.
        /// If value is set to false, PrepareRequest and ProcessResponse methods will be partial methods, and implement them is optional.
        /// </summary>
        public bool GeneratePrepareRequestAndProcessResponseAsAsyncMethods { get; set; }

        /// <summary>Gets or sets a value indicating whether to generate different request and response serialization settings (default: false).</summary>
        public bool UseRequestAndResponseSerializationSettings { get; set; }

        /// <summary>Gets or sets a value indicating whether to serialize the type information in a $type property (not recommended, also sets TypeNameHandling = Auto).</summary>
        public bool SerializeTypeInformation { get; set; }

        /// <summary>Gets or sets the null value used for query parameters which are null (default: '').</summary>
        public string QueryNullValue { get; set; }

        /// <summary>Gets or sets a value indicating whether to expose the JsonSerializerSettings property (default: false).</summary>
        public bool ExposeJsonSerializerSettings { get; set; }
    }
}
