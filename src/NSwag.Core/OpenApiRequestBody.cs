//-----------------------------------------------------------------------
// <copyright file="OpenApiRequestBody.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Specialized;
using System.Text.Json.Serialization;
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
        internal readonly ObservableDictionary<string, OpenApiMediaType> _content;

        /// <summary>Initializes a new instance of the <see cref="OpenApiRequestBody"/> class.</summary>
        public OpenApiRequestBody()
        {
            var content = new ObservableDictionary<string, OpenApiMediaType>();
            content.CollectionChanged += (sender, args) =>
            {
                if (args.Action != NotifyCollectionChangedAction.Add && args.Action != NotifyCollectionChangedAction.Replace)
                {
                    return;
                }

                for (var i = 0; i < args.NewItems.Count; i++)
                {
                    var pair = (KeyValuePair<string, OpenApiMediaType>)args.NewItems[i];
                    pair.Value.Parent = this;
                }

                ParentOperation?.UpdateBodyParameter();
            };

            _content = content;
        }

        /// <summary>Gets or sets the referenced object.</summary>
        [JsonIgnore]
        public override OpenApiRequestBody Reference
        {
            get => base.Reference;
            set
            {
                base.Reference = value;
                ParentOperation?.UpdateBodyParameter();
            }
        }

        [JsonIgnore]
        internal object Parent { get; set; }

        [JsonIgnore]
        internal OpenApiOperation ParentOperation => Parent as OpenApiOperation;

        /// <summary>Gets the actual request body, either this or the referenced request body.</summary>
        [JsonIgnore]
        public OpenApiRequestBody ActualRequestBody => Reference ?? this;

        /// <summary>Gets or sets the name.</summary>
        [JsonPropertyName("x-name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                ParentOperation?.UpdateBodyParameter();
            }
        }

        /// <summary>Gets or sets the description.</summary>
        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                ParentOperation?.UpdateBodyParameter();
            }
        }

        /// <summary>Gets or sets the descriptions of potential response payloads (OpenApi only).</summary>
        [JsonPropertyName("content")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IDictionary<string, OpenApiMediaType> Content => _content;

        /// <summary>Gets or sets the example's external value.</summary>
        [JsonPropertyName("required")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool IsRequired
        {
            get => _isRequired;
            set
            {
                _isRequired = value;
                ParentOperation?.UpdateBodyParameter();
            }
        }

        /// <summary>Gets or sets the name.</summary>
        [JsonPropertyName("x-position")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? Position
        {
            get => _position;
            set
            {
                _position = value;
                ParentOperation?.UpdateBodyParameter();
            }
        }

        /// <summary>Gets the actual name of the request body parameter.</summary>
        [JsonIgnore]
        public string ActualName => string.IsNullOrEmpty(Name) ? "body" : Name;

        #region Implementation of IJsonReference

        [JsonIgnore]
        IJsonReference IJsonReference.ActualObject => ActualRequestBody;

        [JsonIgnore]
        object IJsonReference.PossibleRoot => ParentOperation?.Parent?.Parent;

        #endregion
    }
}