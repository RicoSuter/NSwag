//-----------------------------------------------------------------------
// <copyright file="IgnorableSerializerContractResolver.cs" company="NSwag">
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

namespace NSwag
{
    /// <summary>Special JsonConvert resolver that allows you to ignore properties. See http://stackoverflow.com/a/13588192/1037948 </summary>
    internal class IgnorableSerializerContractResolver : DefaultContractResolver
    {
        protected readonly Dictionary<Type, HashSet<string>> Ignores;

        /// <summary>Initializes a new instance of the <see cref="IgnorableSerializerContractResolver"/> class.</summary>
        public IgnorableSerializerContractResolver()
        {
            Ignores = new Dictionary<Type, HashSet<string>>();
        }

        /// <summary>Explicitly ignore the given property(s) for the given type</summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyNames">One or more properties to ignore. Leave empty to ignore the type entirely.</param>
        public void Ignore(Type type, params string[] propertyNames)
        {
            if (!Ignores.ContainsKey(type))
                Ignores[type] = new HashSet<string>();

            foreach (var prop in propertyNames)
                Ignores[type].Add(prop);
        }

        /// <summary>Is the given property for the given type ignored?</summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public bool IsIgnored(Type type, string propertyName)
        {
            if (!Ignores.ContainsKey(type))
                return false;

            if (Ignores[type].Count == 0)
                return true;

            return Ignores[type].Contains(propertyName);
        }

        /// <summary>The decision logic goes here</summary>
        /// <param name="member">The member to create a <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for.</param>
        /// <param name="memberSerialization">The member's parent <see cref="T:Newtonsoft.Json.MemberSerialization" />.</param>
        /// <returns>A created <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for the given <see cref="T:System.Reflection.MemberInfo" />.</returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (IsIgnored(property.DeclaringType, property.PropertyName))
                property.ShouldSerialize = instance => false;

            return property;
        }
    }
}