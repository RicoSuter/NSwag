//-----------------------------------------------------------------------
// <copyright file="OpenApiRequestBody.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;
using NJsonSchema.References;
using NSwag.Collections;

namespace NSwag
{
    /// <summary>The OpenApi request body (OpenAPI only).</summary>
    public class OpenApiRequestBody : JsonReferenceBase<OpenApiRequestBody>, IJsonReference
    {
        private string _name;
        private bool _isRequired;
        private string _description;
        private int? _position;

        /// <summary>Initializes a new instance of the <see cref="OpenApiRequestBody"/> class.</summary>
        public OpenApiRequestBody()
        {
            var content = new ObservableDictionary<string, OpenApiMediaType>();
            content.CollectionChanged += (sender, args) =>
            {
                foreach (var mediaType in content.Values)
                {
                    mediaType.Parent = this;
                }

                Parent?.UpdateBodyParameter();
            };
            Content = content;
        }

        [JsonIgnore]
        internal OpenApiOperation Parent { get; set; }

        /// <summary>Gets the actual request body, either this or the referenced request body.</summary>
        [JsonIgnore]
        public OpenApiRequestBody ActualRequestBody => Reference ?? this;

        /// <summary>Gets or sets the name.</summary>
        [JsonProperty(PropertyName = "x-name", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                Parent?.UpdateBodyParameter();
            }
        }

        /// <summary>Gets or sets the description.</summary>
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                Parent?.UpdateBodyParameter();
            }
        }

        /// <summary>Gets or sets the descriptions of potential response payloads (OpenApi only).</summary>
        [JsonProperty(PropertyName = "content", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, OpenApiMediaType> Content { get; }

        /// <summary>Gets or sets the example's external value.</summary>
        [JsonProperty(PropertyName = "required", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool IsRequired
        {
            get => _isRequired;
            set
            {
                _isRequired = value;
                Parent?.UpdateBodyParameter();
            }
        }

        /// <summary>Gets or sets the name.</summary>
        [JsonProperty(PropertyName = "x-position", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int? Position
        {
            get => _position;
            set
            {
                _position = value;
                Parent?.UpdateBodyParameter();
            }
        }

        /// <summary>Gets the actual name of the request body parameter.</summary>
        [JsonIgnore]
        public string ActualName => string.IsNullOrEmpty(Name) ? "body" : Name;
 
        #region Implementation of IJsonReference

        [JsonIgnore]
        IJsonReference IJsonReference.ActualObject => ActualRequestBody;

        [JsonIgnore]
        object IJsonReference.PossibleRoot => Parent?.Parent?.Parent;

        #endregion
    }
}