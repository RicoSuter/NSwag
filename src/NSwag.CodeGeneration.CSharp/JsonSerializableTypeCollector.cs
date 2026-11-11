//-----------------------------------------------------------------------
// <copyright file="JsonSerializableTypeCollector.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag.CodeGeneration.CSharp.Models;

namespace NSwag.CodeGeneration.CSharp
{
    /// <summary>Collects the set of CLR types that must appear on the generated <c>JsonSerializerContext</c>
    /// partial class as <c>[JsonSerializable]</c> entries, and produces a stable property name for each so
    /// call sites can reference <c>ContextClassName.Default.&lt;PropertyName&gt;</c>.</summary>
    internal static class JsonSerializableTypeCollector
    {
        private static readonly Dictionary<string, string> _keywordAliases = new(StringComparer.Ordinal)
        {
            ["string"] = "String",
            ["int"] = "Int32",
            ["long"] = "Int64",
            ["short"] = "Int16",
            ["bool"] = "Boolean",
            ["double"] = "Double",
            ["float"] = "Single",
            ["decimal"] = "Decimal",
            ["byte"] = "Byte",
            ["sbyte"] = "SByte",
            ["uint"] = "UInt32",
            ["ulong"] = "UInt64",
            ["ushort"] = "UInt16",
            ["char"] = "Char",
            ["object"] = "Object",
        };

        /// <summary>Walks every operation's request body, object-typed form parameters, and response payloads
        /// (including the default response) to build a deduplicated list of types that need source-generated
        /// JSON metadata.</summary>
        public static List<JsonSerializableTypeModel> Collect(IEnumerable<CSharpOperationModel> operations)
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var result = new List<JsonSerializableTypeModel>();
            var needsFormUrlEncodedDictionary = false;

            foreach (var op in operations)
            {
                if (op.HasContent && !op.HasBinaryBodyParameter && !op.HasXmlBodyParameter && !op.HasPlainTextBodyParameter)
                {
                    AddType(op.ContentParameter?.Type);

                    if (op.ConsumesOnlyFormUrlEncoded)
                    {
                        needsFormUrlEncodedDictionary = true;
                    }
                }

                foreach (var formParam in op.FormParameters)
                {
                    if (formParam.IsObject)
                    {
                        AddType(formParam.Type);
                    }
                }

                foreach (var resp in op.Responses)
                {
                    if (resp.HasType && !resp.IsFile && !resp.IsPlainText)
                    {
                        AddType(resp.Type);
                    }
                }

                if (op.HasDefaultResponse && op.DefaultResponse.HasType
                    && !op.DefaultResponse.IsFile && !op.DefaultResponse.IsPlainText)
                {
                    AddType(op.DefaultResponse.Type);
                }
            }

            if (needsFormUrlEncodedDictionary)
            {
                AddType("System.Collections.Generic.Dictionary<string, string>");
            }

            return result;

            void AddType(string typeRef)
            {
                if (string.IsNullOrWhiteSpace(typeRef) || typeRef == "void")
                {
                    return;
                }

                var normalized = StripTrailingNullable(typeRef.Trim());
                if (!seen.Add(normalized))
                {
                    return;
                }

                result.Add(new JsonSerializableTypeModel(normalized, ToTypeInfoPropertyName(normalized)));
            }
        }

        /// <summary>Maps a C# type reference (e.g. <c>System.Collections.Generic.List&lt;Foo.Bar.Person&gt;</c>)
        /// to a stable property name on the source-generated context (e.g. <c>ListOfPerson</c>).</summary>
        public static string ToTypeInfoPropertyName(string typeRef)
        {
            typeRef = StripTrailingNullable((typeRef ?? string.Empty).Trim());

            if (typeRef.EndsWith("[]", StringComparison.Ordinal))
            {
                var inner = typeRef.Substring(0, typeRef.Length - 2);
                return "ArrayOf" + ToTypeInfoPropertyName(inner);
            }

            var ltIndex = typeRef.IndexOf('<');
            if (ltIndex > 0 && typeRef.Length > 0 && typeRef[typeRef.Length - 1] == '>')
            {
                var outerRaw = typeRef.Substring(0, ltIndex);
                var inner = typeRef.Substring(ltIndex + 1, typeRef.Length - ltIndex - 2);
                var args = SplitTopLevelCommas(inner);
                var outerName = SimpleName(outerRaw);
                var argNames = args.Select(a => ToTypeInfoPropertyName(a.Trim()));
                return outerName + "Of" + string.Join("And", argNames);
            }

            return SimpleName(typeRef);
        }

        private static string SimpleName(string typeRef)
        {
            var name = typeRef.Trim();
            var lastDot = name.LastIndexOf('.');
            if (lastDot >= 0)
            {
                name = name.Substring(lastDot + 1);
            }

            if (_keywordAliases.TryGetValue(name, out var alias))
            {
                return alias;
            }

            return name;
        }

        private static string StripTrailingNullable(string typeRef)
        {
            if (typeRef.Length > 0 && typeRef[typeRef.Length - 1] == '?')
            {
                return typeRef.Substring(0, typeRef.Length - 1).TrimEnd();
            }

            return typeRef;
        }

        private static List<string> SplitTopLevelCommas(string input)
        {
            var parts = new List<string>();
            var depth = 0;
            var start = 0;
            for (var i = 0; i < input.Length; i++)
            {
                var c = input[i];
                if (c == '<')
                {
                    depth++;
                }
                else if (c == '>')
                {
                    depth--;
                }
                else if (c == ',' && depth == 0)
                {
                    parts.Add(input.Substring(start, i - start));
                    start = i + 1;
                }
            }

            parts.Add(input.Substring(start));
            return parts;
        }
    }
}
