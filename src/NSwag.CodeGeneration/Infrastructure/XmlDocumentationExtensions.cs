//-----------------------------------------------------------------------
// <copyright file="XmlDocumentationExtensions.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;

namespace NSwag.CodeGeneration.Infrastructure
{
    /// <summary>Provides extension methods for reading XML comments from reflected members.</summary>
    public static class XmlDocumentationExtensions
    {
        private static readonly Dictionary<string, XDocument> _cache =
            new Dictionary<string, XDocument>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Returns the contents of the "summary" tag (.NET XML documentation) for the specified member.</summary>
        /// <param name="member">The reflected member.</param>
        /// <returns>The contents of the "summary" tag for the member.</returns>
        public static string GetXmlDocumentation(this MemberInfo member)
        {
            return GetXmlDocumentation(member, GetXmlDocumentationPath(member.Module.Assembly));
        }

        /// <summary>Returns the contents of the "returns" or "param" tag (.NET XML documentation) for the specified parameter.</summary>
        /// <param name="parameter">The reflected parameter or return info.</param>
        /// <returns>The contents of the "returns" or "param" tag.</returns>
        public static string GetXmlDocumentation(this ParameterInfo parameter)
        {
            return GetXmlDocumentation(parameter, GetXmlDocumentationPath(parameter.Member.Module.Assembly));
        }

        /// <summary>Returns the contents of the "summary" tag (.NET XML documentation) for the specified member.</summary>
        /// <param name="member">The reflected member.</param>
        /// <param name="pathToXmlFile">The path to the XML documentation file.</param>
        /// <returns>The contents of the "summary" tag for the member.</returns>
        public static string GetXmlDocumentation(this MemberInfo member, string pathToXmlFile)
        {
            if (pathToXmlFile == null || !File.Exists(pathToXmlFile))
                return string.Empty;

            AssemblyName assemblyName = member.Module.Assembly.GetName();

            if (!_cache.ContainsKey(assemblyName.FullName))
                _cache[assemblyName.FullName] = XDocument.Load(pathToXmlFile);

            return GetXmlDocumentation(member, _cache[assemblyName.FullName]);
        }

        /// <summary>Returns the contents of the "returns" or "param" tag (.NET XML documentation) for the specified parameter.</summary>
        /// <param name="parameter">The reflected parameter or return info.</param>
        /// <param name="pathToXmlFile">The path to the XML documentation file.</param>
        /// <returns>The contents of the "returns" or "param" tag.</returns>
        public static string GetXmlDocumentation(this ParameterInfo parameter, string pathToXmlFile)
        {
            if (pathToXmlFile == null || !File.Exists(pathToXmlFile))
                return string.Empty;

            AssemblyName assemblyName = parameter.Member.Module.Assembly.GetName();

            if (!_cache.ContainsKey(assemblyName.FullName))
                _cache[assemblyName.FullName] = XDocument.Load(pathToXmlFile);

            return GetXmlDocumentation(parameter, _cache[assemblyName.FullName]);
        }

        /// <summary>Returns the contents of the "summary" tag (.NET XML documentation) for the specified member.</summary>
        /// <param name="member">The reflected member.</param>
        /// <param name="xml">The XML documentation document.</param>
        /// <returns>The contents of the "summary" tag for the member.</returns>
        public static string GetXmlDocumentation(this MemberInfo member, XDocument xml)
        {
            var name = GetMemberElementName(member); 
            return xml.XPathEvaluate(string.Format("string(/doc/members/member[@name='{0}']/summary)",name)).ToString().Trim();
        }

        /// <summary>Returns the contents of the "returns" or "param" tag (.NET XML documentation) for the specified parameter.</summary>
        /// <param name="parameter">The reflected parameter or return info.</param>
        /// <param name="xml">The XML documentation document.</param>
        /// <returns>The contents of the "returns" or "param" tag.</returns>
        public static string GetXmlDocumentation(this ParameterInfo parameter, XDocument xml)
        {
            var name = GetMemberElementName(parameter.Member); 
            if (parameter.IsRetval || string.IsNullOrEmpty(parameter.Name))
                return xml.XPathEvaluate(string.Format("string(/doc/members/member[@name='{0}']/returns)", name)).ToString().Trim();
            else
                return xml.XPathEvaluate(string.Format("string(/doc/members/member[@name='{0}']/param[@name='{1}'])", name, parameter.Name)).ToString().Trim();
        }

        private static string GetMemberElementName(MemberInfo member)
        {
            char prefixCode;
            string memberName = (member is Type)
                ? ((Type)member).FullName                               // member is a Type
                : (member.DeclaringType.FullName + "." + member.Name);  // member belongs to a Type

            switch (member.MemberType)
            {
                case MemberTypes.Constructor:
                    // XML documentation uses slightly different constructor names
                    memberName = memberName.Replace(".ctor", "#ctor");
                    goto case MemberTypes.Method;

                case MemberTypes.Method:
                    prefixCode = 'M';

                    // parameters are listed according to their type, not their name
                    string paramTypesList = String.Join(
                        ",",
                        ((MethodBase)member).GetParameters()
                            .Cast<ParameterInfo>()
                            .Select(x => x.ParameterType.FullName
                            ).ToArray()
                        );
                    if (!String.IsNullOrEmpty(paramTypesList)) memberName += "(" + paramTypesList + ")";
                    break;

                case MemberTypes.Event:
                    prefixCode = 'E';
                    break;

                case MemberTypes.Field:
                    prefixCode = 'F';
                    break;

                case MemberTypes.NestedType:
                    // XML documentation uses slightly different nested type names
                    memberName = memberName.Replace('+', '.');
                    goto case MemberTypes.TypeInfo;

                case MemberTypes.TypeInfo:
                    prefixCode = 'T';
                    break;

                case MemberTypes.Property:
                    prefixCode = 'P';
                    break;

                default:
                    throw new ArgumentException("Unknown member type", "member");
            }

            // elements are of the form "M:Namespace.Class.Method"
            return string.Format("{0}:{1}", prefixCode, memberName);
        }
        
        private static string GetXmlDocumentationPath(Assembly assembly)
        {
            var assemblyName = assembly.GetName();

            var path = Path.Combine(Path.GetDirectoryName(assembly.Location), assemblyName.Name + ".xml");
            if (File.Exists(path))
                return path;

            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyName.Name + ".xml");
            if (File.Exists(path))
                return path;

            return null;
        }
    }
}