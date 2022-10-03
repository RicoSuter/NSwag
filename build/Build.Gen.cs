using Nuke.Common;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System;
using NSwag.Commands.CodeGeneration;
using NConsole;
using System.Diagnostics.CodeAnalysis;
using Nuke.Common.IO;

public partial class Build
{
    private AbsolutePath ApiDescriptionClientDirectory => SourceDirectory / "NSwag.ApiDescription.Client";
    private AbsolutePath NSwagMSBuildDirectory => SourceDirectory / "NSwag.MSBuild";
    private AbsolutePath TemplatesDirectory => BuildProjectDirectory / "templates";

    private const string NSwagEnablePropertySchema = nameof(NSwagEnablePropertySchema);

    Target GeneratePageSchema => _ => _
        .DependentFor(Pack)
        .Executes(() =>
        {
            Helpers.Generate<OpenApiToCSharpClientCommand>(TemplatesDirectory, ApiDescriptionClientDirectory, "OpenApiReference", "CodeGen", Lang.CSharp);
            Helpers.Generate<OpenApiToTypeScriptClientCommand>(TemplatesDirectory, ApiDescriptionClientDirectory, "OpenApiReference", "CodeGen", Lang.TypeScript);
            Helpers.Generate<OpenApiToCSharpControllerCommand>(TemplatesDirectory, NSwagMSBuildDirectory, "NSwagController", "CodeGen", Lang.CSharp);
        });

    #region GenerateHelpers

    enum Lang { CSharp, TypeScript }

    private static class Helpers
    {
        public static void Generate<T>(AbsolutePath templateDirectory, AbsolutePath outputDirectory, string itemName, string schemaCategory, Lang lang)
        {
            var options = GetOptions<T>();

            var template = XDocument.Load(templateDirectory / $"{itemName}ExtensionSchema.xml");
            InjectOpenApiOptions(template, schemaCategory, itemName, options);

            var optionsUpdate = CreateItemUpdate(itemName, options);

            template.Save(outputDirectory / $"{itemName}ExtensionSchema{lang}.xml");
            optionsUpdate.Save(outputDirectory / $"{itemName}Update{lang}.targets");
        }


        private static Option[] GetOptions<T>()
        {
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var options = new List<Option>(properties.Length);
            foreach (var property in properties)
            {
                var argument = property.GetCustomAttribute<ArgumentAttribute>();
                if (Skip<T>(argument)) continue;

                options.Add(new Option(argument.Name, property.PropertyType, argument.Description));
            }

            return options.ToArray();
        }

        private static bool Skip<T>(ArgumentAttribute attribute)
        {
            if (attribute == null)
                return true;

            switch (typeof(T))
            {
                case { Name: nameof(OpenApiToCSharpClientCommand) }:
                case { Name: nameof(OpenApiToTypeScriptClientCommand) }:
                    return _clientSkip.Contains(attribute.Name);
                case { Name: nameof(OpenApiToCSharpControllerCommand) }:
                    return _controllerSkip.Contains(attribute.Name);
                default:
                    throw new InvalidOperationException($"Unexpected type {typeof(T).Name}");
            }

        }

        private static void InjectOpenApiOptions(XDocument template, string category, string itemName, IEnumerable<Option> options)
        {
            var content = MapToElement(options, category, itemName).Where(x => x != null);
            template.Element(GetName("ProjectSchemaDefinitions")).Element(GetName("Rule")).Add(content);
        }

        private static XElement CreateItemUpdate(string name, IEnumerable<Option> options)
        {
            var content = new List<XObject>()
            {
                new XAttribute("Update", $"@({name})")
            };

            foreach (var option in options)
            {
                var metadata = new XElement(XName.Get("Options"));
                metadata.Add(new XAttribute("Condition", $"'%({option.Name})' != ''"));
                metadata.Add(new XText($"%(Options) /{option.Name}:%({option.Name})"));
                content.Add(metadata);
            }

            return new XElement(XName.Get("Project"),
                new XElement(XName.Get("ItemGroup"),
                    new XElement(name, content.ToArray())));
        }

        private static IEnumerable<XElement> MapToElement(IEnumerable<Option> options, string category, string itemName)
        {
            foreach (var option in options)
            {
                var name = option.Name;
                var type = option.Type;
                var description = option.Description;

                var attributes = CreateCommonAttributes(name, name, description, category);

                if (type == typeof(string))
                    yield return CreateProperty(StringProperty, itemName, attributes);
                else if (type.IsEnum)
                    yield return CreateProperty(EnumProperty, itemName, attributes.Concat(CreateEnumValues(type)));
                else if (type.IsArray)
                    yield return CreateProperty(StringListProperty, itemName, attributes);
                else if (type == typeof(bool))
                    yield return CreateProperty(BoolProperty, itemName, attributes.Append(CreateDefaultAttribute(description)));
                else if (type == typeof(decimal))
                    yield return CreateProperty(IntProperty, itemName, attributes);
                else
                    throw new InvalidOperationException($"Unknown type {type}");
            }
        }

        private static XAttribute[] CreateCommonAttributes(string name, string displayName, string description, string category)
        {
            return new[]
            {
            new XAttribute("Name", name),
            new XAttribute("DisplayName", displayName),
            new XAttribute("Description", description),
            new XAttribute("Category", category),
        };
        }

        private static XElement CreateProperty(string typeName, string itemName, IEnumerable<object> content)
        {
            return new XElement(GetName(typeName), content.Append(CreateDataSource(typeName, itemName)).ToArray());
        }

        private static XAttribute CreateDefaultAttribute(string description)
        {
            if (description.Contains("default: true"))
                return new XAttribute("Default", "true");
            if (description.Contains("default: false"))
                return new XAttribute("Default", "false");
            return null;
        }

        static IEnumerable<XObject> CreateEnumValues(Type enumType)
        {
            // first is default
            var first = true;
            foreach (var value in Enum.GetNames(enumType))
            {
                var enumValue = new XElement(GetName("EnumValue"), new XAttribute("Name", value));
                if (first)
                    enumValue.Add(new XAttribute("IsDefault", "true"));

                yield return enumValue;
                first = false;
            };
        }

        private static XElement CreateDataSource(string propertyName, string itemType)
        {
            return new XElement(GetName($"{propertyName}.DataSource"),
                    new XElement(GetName("DataSource"),
                        new XAttribute("ItemType", itemType),
                        new XAttribute("HasConfigurationCondition", "false"),
                        new XAttribute("SourceOfDefaultValue", "AfterContext")));
        }

        private static XName GetName(string name) => XName.Get(name, Namespace);

        public class Option
        {
            public Option(string name, Type type, string description)
            {
                Name = name ?? throw new ArgumentNullException(nameof(name));
                Type = type ?? throw new ArgumentNullException(nameof(type));
                Description = description ?? throw new ArgumentNullException(nameof(description));
            }

            public string Name { get; set; }
            public Type Type { get; set; }
            public string Description { get; set; }


            public static IEqualityComparer<Option> Comparer { get; } = new OptionComparer();

            private class OptionComparer : IEqualityComparer<Option>
            {
                public bool Equals(Option x, Option y)
                {
                    return string.Equals(x.Name, y.Name);
                }

                public int GetHashCode([DisallowNull] Option obj)
                {
                    return obj.Name.GetHashCode();
                }
            }
        }

        private static readonly string[] _clientSkip = new[] { "Input", "Output", "ClassName", "Namespace", "GenerateExceptionClasses" };
        private static readonly string[] _controllerSkip = new[] { "Input", "Output" };

        private const string Namespace = "http://schemas.microsoft.com/build/2009/properties";
        private const string StringProperty = nameof(StringProperty);
        private const string StringListProperty = nameof(StringListProperty);
        private const string EnumProperty = nameof(EnumProperty);
        private const string BoolProperty = nameof(BoolProperty);
        private const string IntProperty = nameof(IntProperty);
    }

    #endregion
}
