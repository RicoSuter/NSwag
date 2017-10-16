//-----------------------------------------------------------------------
// <copyright file="AssemblyTypeToSwaggerCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using NConsole;
using Newtonsoft.Json;
using NJsonSchema;
using NSwag.SwaggerGeneration;

namespace NSwag.Commands
{
    /// <summary></summary>
    [Command(Name = "types2swagger")]
    public class AssemblyTypeToSwaggerCommand : AssemblyOutputCommandBase<AssemblyTypeToSwaggerGenerator>
    {
        /// <summary>Initializes a new instance of the <see cref="AssemblyTypeToSwaggerCommand"/> class.</summary>
        public AssemblyTypeToSwaggerCommand()
            : base(new AssemblyTypeToSwaggerGeneratorSettings())
        {
            ClassNames = new string[] { };
        }

        [JsonIgnore]
        public new AssemblyTypeToSwaggerGeneratorSettings Settings => (AssemblyTypeToSwaggerGeneratorSettings)base.Settings;

        [Argument(Name = "Assembly", IsRequired = true, Description = "The path to the Web API .NET assembly.")]
        public string[] AssemblyPaths
        {
            get { return Settings.AssemblySettings.AssemblyPaths; }
            set { Settings.AssemblySettings.AssemblyPaths = value; }
        }

        [Argument(Name = "ClassNames", Description = "The class names.")]
        public string[] ClassNames { get; set; }

        [Argument(Name = "DefaultPropertyNameHandling", IsRequired = false, Description = "The default property name handling ('Default' or 'CamelCase').")]
        public PropertyNameHandling DefaultPropertyNameHandling
        {
            get { return Settings.DefaultPropertyNameHandling; }
            set { Settings.DefaultPropertyNameHandling = value; }
        }

        [Argument(Name = "DefaultReferenceTypeNullHandling", IsRequired = false, Description = "The default null handling (if NotNullAttribute and CanBeNullAttribute are missing, default: Null, Null or NotNull).")]
        public ReferenceTypeNullHandling DefaultReferenceTypeNullHandling
        {
            get { return Settings.DefaultReferenceTypeNullHandling; }
            set { Settings.DefaultReferenceTypeNullHandling = value; }
        }

        [Argument(Name = "DefaultEnumHandling", IsRequired = false, Description = "The default enum handling ('String' or 'Integer'), default: Integer.")]
        public EnumHandling DefaultEnumHandling
        {
            get { return Settings.DefaultEnumHandling; }
            set { Settings.DefaultEnumHandling = value; }
        }

        [Argument(Name = "FlattenInheritanceHierarchy", IsRequired = false, Description = "Flatten the inheritance hierarchy instead of using allOf to describe inheritance (default: false).")]
        public bool FlattenInheritanceHierarchy
        {
            get { return Settings.FlattenInheritanceHierarchy; }
            set { Settings.FlattenInheritanceHierarchy = value; }
        }

        [Argument(Name = "GenerateKnownTypes", IsRequired = false, Description = "Generate schemas for types in KnownTypeAttribute attributes (default: true).")]
        public bool GenerateKnownTypes
        {
            get { return Settings.GenerateKnownTypes; }
            set { Settings.GenerateKnownTypes = value; }
        }

        [Argument(Name = "GenerateXmlObjects", IsRequired = false, Description = "Generate xmlObject representation for definitions (default: false).")]
        public bool GenerateXmlObjects
        {
            get { return Settings.GenerateXmlObjects; }
            set { Settings.GenerateXmlObjects = value; }
        }

        public override async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var document = await RunAsync();
            await TryWriteDocumentOutputAsync(host, () => document).ConfigureAwait(false);
            return document;
        }

        public async Task<SwaggerDocument> RunAsync()
        {
            return await Task.Run(async () =>
            {
                var generator = new AssemblyTypeToSwaggerGenerator(Settings);
                var document = await generator.GenerateAsync(ClassNames).ConfigureAwait(false);
#if DEBUG
                var json = document.ToJson();
#endif
                return document;
            });
        }
    }
}