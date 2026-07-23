using NJsonSchema;

namespace NSwag.CodeGeneration.CSharp
{
    /// <summary>Generates a CSharp type name with optional prefix and suffix.</summary>
    public class CSharpTypeNameGenerator : DefaultTypeNameGenerator
    {
        private readonly string _prefix;
        private readonly string _suffix;

        /// <summary>Initializes a new instance of the <see cref="CSharpTypeNameGenerator"/> class.</summary>
        /// <param name="prefix">The prefix to prepend to generated type names.</param>
        /// <param name="suffix">The suffix to append to generated type names.</param>
        public CSharpTypeNameGenerator(string prefix = "", string suffix = "")
        {
            _prefix = prefix ?? string.Empty;
            _suffix = suffix ?? string.Empty;
        }

        /// <summary>Generates the type name for the given schema.</summary>
        /// <param name="schema">The schema.</param>
        /// <param name="typeNameHint">The type name hint.</param>
        /// <returns>The type name.</returns>
        protected override string Generate(JsonSchema schema, string typeNameHint)
        {
            var name = base.Generate(schema, typeNameHint);

            if (!schema.ActualSchema.IsEnumeration && (!string.IsNullOrEmpty(_prefix) || !string.IsNullOrEmpty(_suffix)))
            {
                return _prefix + name + _suffix;
            }

            return name;
        }
    }
}
