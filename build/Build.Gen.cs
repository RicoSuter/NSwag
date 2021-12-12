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
    private AbsolutePath TemplatesDirectory => BuildProjectDirectory / "templates";

    Target GeneratePageSchema => _ => _
        .DependentFor(Pack)
        .Executes(() =>
        {
            var csharpOptions = GetOptions<OpenApiToCSharpClientCommand>();
            var typeScriptOptions = GetOptions<OpenApiToTypeScriptClientCommand>();

            // Property Page Schema
            var template = XDocument.Load(TemplatesDirectory / "OpenApiItemsExtensionSchema.xml");
            InjectOptions(template, csharpOptions, typeScriptOptions);
            template.Save(ApiDescriptionClientDirectory / "OpenApiItemsExtensionSchema.xml");

            // OpenApiReference Update
            var openApiReferenceUpdate = CreateOpenApiReferenceUpdate(csharpOptions, typeScriptOptions);
            openApiReferenceUpdate.Save(ApiDescriptionClientDirectory / "OpenApiReferenceUpdate.targets");
        });

    private static Option[] GetOptions<T>()
    {
        var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
        var options = new List<Option>(properties.Length);
        foreach (var property in properties)
        {
            var argument = property.GetCustomAttribute<ArgumentAttribute>();
            if (argument == null || _skip.Contains(argument.Name)) continue;

            options.Add(new Option(argument.Name, property.PropertyType, argument.Description));
        }

        return options.ToArray();
    }

    private static void InjectOptions(XDocument template, IEnumerable<Option> csharpOptions, IEnumerable<Option> typeScriptOptions)
    {
        var content = CreatePropertyPageContent(csharpOptions, typeScriptOptions);
        template.Element(GetName("ProjectSchemaDefinitions")).Element(GetName("Rule")).Add(content);
    }

    private static IEnumerable<XElement> CreatePropertyPageContent(IEnumerable<Option> csharpOptions, IEnumerable<Option> typeScriptOptions)
    {
        var commonOptions = csharpOptions.Intersect(typeScriptOptions, Option.Comparer);
        var onlyCsharpOptions = csharpOptions.Except(typeScriptOptions, Option.Comparer);
        var onlyTypeScriptOptions = typeScriptOptions.Except(csharpOptions, Option.Comparer);

        return MapToElement(commonOptions, "NSwagCodeGen")
            .Concat(MapToElement(onlyCsharpOptions, "NSwagCSharpCodeGen"))
            .Concat(MapToElement(onlyTypeScriptOptions, "NSwagTypeScriptCodeGen"))
            .Where(x => x != null);

        static IEnumerable<XElement> MapToElement(IEnumerable<Option> options, string category)
        {
            foreach (var option in options)
            {
                var name = option.Name;
                var type = option.Type;
                var description = option.Description;

                var attributes = CreateCommonAttributes(name, name, description, category);

                if (type == typeof(string))
                    yield return CreateProperty(StringProperty, attributes);
                else if (type.IsEnum)
                    yield return CreateProperty(EnumProperty, attributes.Concat(CreateEnumValues(type)));
                else if (type.IsArray)
                    yield return CreateProperty(StringListProperty, attributes);
                else if (type == typeof(bool))
                    yield return CreateProperty(BoolProperty, attributes.Append(CreateDefaultAttribute(description)));
                else if (type == typeof(decimal))
                    yield return CreateProperty(IntProperty, attributes);
                else
                    throw new InvalidOperationException($"Unknown type {type}");
            }
        }
    }

    private static XElement CreateOpenApiReferenceUpdate(IEnumerable<Option> csharpOptions, IEnumerable<Option> typeScriptOptions)
    {
        var content = new List<XObject>()
            {
                new XAttribute("Update", "@(OpenApiReference)")
            };

        foreach (var option in csharpOptions.Concat(typeScriptOptions).Distinct(Option.Comparer))
        {
            var metadata = new XElement(XName.Get("Options"));
            metadata.Add(new XAttribute("Condition", $"'%({option.Name})' != ''"));
            metadata.Add(new XText($"%(Options) /{option.Name}:%({option.Name})"));
            content.Add(metadata);
        }

        return new XElement(XName.Get("Project"),
            new XElement(XName.Get("ItemGroup"),
                new XElement("OpenApiReference", content.ToArray())));
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

    private static XElement CreateProperty(string typeName, IEnumerable<object> content)
    {
        return new XElement(GetName(typeName), content.Append(CreateDataSource(typeName)).ToArray());
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

    private static XElement CreateDataSource(string propertyName)
    {
        return new XElement(GetName($"{propertyName}.DataSource"),
                new XElement(GetName("DataSource"),
                    new XAttribute("ItemType", "OpenApiReference"),
                    new XAttribute("HasConfigurationCondition", "false"),
                    new XAttribute("SourceOfDefaultValue", "AfterContext")));
    }

    private static XName GetName(string name) => XName.Get(name, Namespace);

    class Option
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


        public static IEqualityComparer<Option> Comparer { get; } = new _Comparer();

        private class _Comparer : IEqualityComparer<Option>
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

    private static string[] _skip = new[] { "Input", "Output", "ClassName", "Namespace", "GenerateExceptionClasses" };

    private const string Namespace = "http://schemas.microsoft.com/build/2009/properties";
    private const string StringProperty = nameof(StringProperty);
    private const string StringListProperty = nameof(StringListProperty);
    private const string EnumProperty = nameof(EnumProperty);
    private const string BoolProperty = nameof(BoolProperty);
    private const string IntProperty = nameof(IntProperty);
}
