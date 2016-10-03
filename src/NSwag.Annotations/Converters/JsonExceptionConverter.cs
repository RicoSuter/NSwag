//-----------------------------------------------------------------------
// <copyright file="JsonExceptionConverter.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace NSwag.Annotations.Converters
{
    // IMPORTANT: Always sync with JsonExceptionConverterTemplate.tt, IMPORTANT: Copy from CanWrite property

    /// <summary>A converter to correctly serialize exception objects.</summary>
    public class JsonExceptionConverter : JsonConverter
    {
        private readonly DefaultContractResolver _defaultContractResolver = new DefaultContractResolver();
        private readonly IDictionary<string, Assembly> _searchedNamespaces;
        private readonly bool _hideStackTrace;

        /// <summary>Initializes a new instance of the <see cref="JsonExceptionConverter"/> class.</summary>
        public JsonExceptionConverter() : this(true, new Dictionary<string, Assembly>())
        {
        }

        /// <summary>Initializes a new instance of the <see cref="JsonExceptionConverter" /> class.</summary>
        /// <param name="hideStackTrace">If set to <c>true</c> the serializer hides stack trace (i.e. sets the StackTrace to 'HIDDEN').</param>
        /// <param name="searchedNamespaces">The namespaces to search for exception types.</param>
        public JsonExceptionConverter(bool hideStackTrace, IDictionary<string, Assembly> searchedNamespaces)
        {
            _hideStackTrace = hideStackTrace; 
            _searchedNamespaces = searchedNamespaces;
        }

        /// <summary>Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can write JSON.</summary>
        public override bool CanWrite => true;

        /// <summary>Writes the JSON representation of the object.</summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var exception = value as Exception;
            if (exception != null)
            {
                var resolver = serializer.ContractResolver as DefaultContractResolver ?? _defaultContractResolver;

                var jObject = new JObject();
                jObject.Add(resolver.GetResolvedPropertyName("discriminator"), exception.GetType().Name);
                jObject.Add(resolver.GetResolvedPropertyName("Message"), exception.Message);
                jObject.Add(resolver.GetResolvedPropertyName("StackTrace"), _hideStackTrace ? "HIDDEN" : exception.StackTrace);
                jObject.Add(resolver.GetResolvedPropertyName("Source"), exception.Source);
                jObject.Add(resolver.GetResolvedPropertyName("InnerException"),
                    exception.InnerException != null ? JToken.FromObject(exception.InnerException, serializer) : null);

                foreach (var property in GetExceptionProperties(value.GetType()))
                {
                    var propertyValue = property.Key.GetValue(exception);
                    if (propertyValue != null)
                    {
                        jObject.AddFirst(new JProperty(resolver.GetResolvedPropertyName(property.Value),
                            JToken.FromObject(propertyValue, serializer)));
                    }
                }

                value = jObject;
            }

            serializer.Serialize(writer, value);
        }

        /// <summary>Determines whether this instance can convert the specified object type.</summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns><c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.</returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(Exception).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        /// <summary>Reads the JSON representation of the object.</summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = serializer.Deserialize<JObject>(reader);
            if (jObject == null)
                return null;

            var newSerializer = new JsonSerializer();
            newSerializer.ContractResolver = (IContractResolver)Activator.CreateInstance(serializer.ContractResolver.GetType());

            GetField(typeof(DefaultContractResolver), "_sharedCache").SetValue(newSerializer.ContractResolver, false);

            dynamic resolver = newSerializer.ContractResolver;
            if (newSerializer.ContractResolver.GetType().GetRuntimeProperty("IgnoreSerializableAttribute") != null)
                resolver.IgnoreSerializableAttribute = true;
            if (newSerializer.ContractResolver.GetType().GetRuntimeProperty("IgnoreSerializableInterface") != null)
                resolver.IgnoreSerializableInterface = true;

            JToken token;
            if (jObject.TryGetValue("discriminator", StringComparison.OrdinalIgnoreCase, out token))
            {
                var discriminator = token.Value<string>();
                if (objectType.Name.Equals(discriminator) == false)
                {
                    var exceptionType = Type.GetType("System." + discriminator, false);
                    if (exceptionType != null)
                        objectType = exceptionType;
                    else
                    {
                        foreach (var pair in _searchedNamespaces)
                        {
                            exceptionType = pair.Value.GetType(pair.Key + "." + discriminator);
                            if (exceptionType != null)
                            {
                                objectType = exceptionType;
                                break;
                            }
                        }

                    }
                }
            }

            var value = jObject.ToObject(objectType, newSerializer);
            foreach (var property in GetExceptionProperties(value.GetType()))
            {
                var jValue = jObject.GetValue(resolver.GetResolvedPropertyName(property.Value));
                var propertyValue = (object)jValue?.ToObject(property.Key.PropertyType);
                if (property.Key.SetMethod != null)
                    property.Key.SetValue(value, propertyValue);
                else
                {
                    var field = GetField(objectType, "m_" + property.Value.Substring(0, 1).ToLowerInvariant() + property.Value.Substring(1));
                    if (field != null)
                        field.SetValue(value, propertyValue);
                }
            }

            SetExceptionFieldValue(jObject, "Message", value, "_message", resolver, newSerializer);
            SetExceptionFieldValue(jObject, "StackTrace", value, "_stackTraceString", resolver, newSerializer);
            SetExceptionFieldValue(jObject, "Source", value, "_source", resolver, newSerializer);
            SetExceptionFieldValue(jObject, "InnerException", value, "_innerException", resolver, serializer);

            return value;
        }

        private FieldInfo GetField(Type type, string fieldName)
        {
            var field = type.GetTypeInfo().GetDeclaredField(fieldName);
            if (field == null && type.GetTypeInfo().BaseType != null)
                return GetField(type.GetTypeInfo().BaseType, fieldName);
            return field;
        }

        private IDictionary<PropertyInfo, string> GetExceptionProperties(Type exceptionType)
        {
            var result = new Dictionary<PropertyInfo, string>();
            foreach (var property in exceptionType.GetRuntimeProperties().Where(p => p.GetMethod?.IsPublic == true))
            {
                var attribute = property.GetCustomAttribute<JsonPropertyAttribute>();
                var propertyName = attribute != null ? attribute.PropertyName : property.Name;

                if (!new[] { "Message", "StackTrace", "Source", "InnerException", "Data", "TargetSite", "HelpLink", "HResult" }.Contains(propertyName))
                    result[property] = propertyName;
            }
            return result;
        }

        private void SetExceptionFieldValue(JObject jObject, string propertyName, object value, string fieldName, IContractResolver resolver, JsonSerializer serializer)
        {
            var field = typeof(Exception).GetTypeInfo().GetDeclaredField(fieldName);
            var jsonPropertyName = resolver is DefaultContractResolver ? ((DefaultContractResolver)resolver).GetResolvedPropertyName(propertyName) : propertyName;
            if (jObject[jsonPropertyName] != null)
            {
                var fieldValue = jObject[jsonPropertyName].ToObject(field.FieldType, serializer);
                field.SetValue(value, fieldValue);
            }
        }
    }
}