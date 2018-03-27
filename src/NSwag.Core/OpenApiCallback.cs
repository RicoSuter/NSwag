//-----------------------------------------------------------------------
// <copyright file="OpenApiCallback.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Newtonsoft.Json;
using NJsonSchema.References;
using System.Collections;
using System.Collections.Generic;

namespace NSwag
{
    /// <summary>Describes an OpenAPI callback.</summary>
    public class OpenApiCallback : JsonReferenceBase<OpenApiCallback>, IJsonReference, IDictionary<string, SwaggerPathItem>
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        private IDictionary<string, SwaggerPathItem> _dictionary = new Dictionary<string, SwaggerPathItem>();

        #region IDictionary

        public SwaggerPathItem this[string key]
        {
            get => _dictionary[key];
            set => _dictionary[key] = value;
        }

        public ICollection<string> Keys => _dictionary.Keys;

        public ICollection<SwaggerPathItem> Values => _dictionary.Values;

        public int Count => _dictionary.Count;

        public bool IsReadOnly => false;

        public void Add(string key, SwaggerPathItem value)
        {
            _dictionary.Add(key, value);
        }

        public void Add(KeyValuePair<string, SwaggerPathItem> item)
        {
            _dictionary.Add(item);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, SwaggerPathItem> item)
        {
            return _dictionary.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, SwaggerPathItem>[] array, int arrayIndex)
        {
            _dictionary.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, SwaggerPathItem>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return _dictionary.Remove(key);
        }

        public bool Remove(KeyValuePair<string, SwaggerPathItem> item)
        {
            return _dictionary.Remove(item);
        }

        public bool TryGetValue(string key, out SwaggerPathItem value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        #endregion

        #region Implementation of IJsonReference

        [JsonIgnore]
        IJsonReference IJsonReference.ActualObject => Reference;

        [JsonIgnore]
        object IJsonReference.PossibleRoot => null;

        #endregion
    }
}