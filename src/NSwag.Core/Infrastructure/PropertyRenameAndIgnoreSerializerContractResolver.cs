//-----------------------------------------------------------------------
// <copyright file="PropertyRenameAndIgnoreSerializerContractResolver.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NSwag.Infrastructure
{
    /// <summary>JsonConvert resolver that allows to ignore and rename properties for given types.</summary>
    internal class PropertyRenameAndIgnoreSerializerContractResolver : DefaultContractResolver
    {
        private readonly Dictionary<Type, HashSet<string>> Ignores;
        private readonly Dictionary<Type, Dictionary<string, string>> Renames;

        /// <summary>Initializes a new instance of the <see cref="PropertyRenameAndIgnoreSerializerContractResolver"/> class.</summary>
        public PropertyRenameAndIgnoreSerializerContractResolver()
        {
            Ignores = new Dictionary<Type, HashSet<string>>();
            Renames = new Dictionary<Type, Dictionary<string, string>>();
        }

        /// <summary>Ignore the given property/properties of the given type.</summary>
        /// <param name="type">The type.</param>
        /// <param name="jsonPropertyNames">One or more JSON properties to ignore.</param>
        public void IgnoreProperty(Type type, params string[] jsonPropertyNames)
        {
            if (!Ignores.ContainsKey(type))
                Ignores[type] = new HashSet<string>();

            foreach (var prop in jsonPropertyNames)
                Ignores[type].Add(prop);
        }

        /// <summary>Rename a property of the given type.</summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">The JSON property name to rename.</param>
        /// <param name="newJsonPropertyName">The new JSON property name.</param>
        public void RenameProperty(Type type, string propertyName, string newJsonPropertyName)
        {
            Dictionary<string, string> renames;

            if (!Renames.TryGetValue(type, out renames))
                Renames[type] = renames = new Dictionary<string, string>();

            renames[propertyName] = newJsonPropertyName;
        }

        /// <summary>Creates a Newtonsoft.Json.Serialization.JsonProperty for the given System.Reflection.MemberInfo.</summary>
        /// <param name="member">The member's parent Newtonsoft.Json.MemberSerialization.</param>
        /// <param name="memberSerialization">The member to create a Newtonsoft.Json.Serialization.JsonProperty for.</param>
        /// <returns>A created Newtonsoft.Json.Serialization.JsonProperty for the given System.Reflection.MemberInfo.</returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (IsIgnored(property.DeclaringType, property.PropertyName))
            {
                property.ShouldSerialize = i => false;
            }

            if (IsRenamed(property.DeclaringType, property.PropertyName, out var newJsonPropertyName))
            {
                property.PropertyName = newJsonPropertyName;
            }

            return property;
        }

        private bool IsIgnored(Type type, string jsonPropertyName)
        {
            if (!Ignores.ContainsKey(type))
                return false;

            return Ignores[type].Contains(jsonPropertyName);
        }

        private bool IsRenamed(Type type, string jsonPropertyName, out string newJsonPropertyName)
        {
            Dictionary<string, string> renames;

            if (!Renames.TryGetValue(type, out renames) || !renames.TryGetValue(jsonPropertyName, out newJsonPropertyName))
            {
                newJsonPropertyName = null;
                return false;
            }

            return true;
        }
    }
}