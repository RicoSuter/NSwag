using System;
using System.Linq;
using System.Reflection;

namespace NConsole
{
    /// <summary>Attribute to define a command line argument.</summary>
    public class ArgumentAttribute : ArgumentAttributeBase
    {
        /// <summary>Gets or sets the argument name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the argument description.</summary>
        public string Description { get; set; }

        /// <summary>Gets or sets the position of the unnamed argument.</summary>
        public int Position { get; set; }

        /// <summary>Gets or sets a value indicating whether the argument is required (default: true).</summary>
        public bool IsRequired { get; set; } = true;

        /// <summary>Gets or sets a value indicating whether the argument accepts an input from a previous command (default: false).</summary>
        public bool AcceptsCommandInput { get; set; }

        /// <summary>Gets or sets a value indicating whether to prompt the user for the value.</summary>
        public bool ShowPrompt { get; set; } = true;

        /// <summary>Gets the argument value.</summary>
        /// <param name="consoleHost">The command line host.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="property">The property.</param>
        /// <param name="command">The command.</param>
        /// <param name="input">The output from the previous command in the chain.</param>
        /// <param name="used">Indicates whether a value for the property was found in the given arguments.</param>
        /// <returns>The value.</returns>
        /// <exception cref="System.InvalidOperationException">Either the argument Name or Position can be set, but not both.</exception>
        /// <exception cref="InvalidOperationException">Either the argument Name or Position can be set, but not both.</exception>
        /// <exception cref="InvalidOperationException">The parameter has no default value.</exception>
        public override object GetValue(IConsoleHost consoleHost, string[] args, PropertyInfo property, IConsoleCommand command, object input, out string used)
        {
            if (!string.IsNullOrEmpty(Name) && Position > 0)
                throw new InvalidOperationException("Either the argument Name or Position can be set, but not both.");

            used = null;
            string value = null;

            if (TryGetPositionalArgumentValue(args, ref used, out value))
            {
                return ArgumentAttributeBase.ConvertToType(value, property.PropertyType);
            }
                
            if (TryGetNamedArgumentValue(args, ref used, out value))
            {
                return ArgumentAttributeBase.ConvertToType(value, property.PropertyType);
            }
                
            if (AcceptsCommandInput && input != null)
                return input;

            if (!ArgumentAttribute.IsInteractiveMode(args) && !IsRequired)
                return property.CanRead ? property.GetValue(command) : null;

            if (ShowPrompt)
            {
                value = consoleHost.ReadValue(GetFullParameterDescription(property, command));
                if (value == "[default]")
                {
                    if (!IsRequired)
                        return property.CanRead ? property.GetValue(command) : null;

                    throw new InvalidOperationException("The parameter '" + Name + "' is required.");
                }

                return ArgumentAttributeBase.ConvertToType(value, property.PropertyType);
            }
            else
                return property.CanRead ? property.GetValue(command) : null;
        }

        private static bool IsInteractiveMode(string[] args)
        {
            return args.Length == 0;
        }

        private bool TryGetPositionalArgumentValue(string[] args, ref string used, out string value)
        {
            if (Position > 0 && Position < args.Length)
            {
                value = args[Position];
                used = value;
                return true;
            }

            value = null;
            return false;
        }

        private bool TryGetNamedArgumentValue(string[] args, ref string used, out string value)
        {
            value = null;

            if (string.IsNullOrEmpty(Name))
                return false;

            var arg = args.FirstOrDefault(a => a.StartsWith("/" + Name.ToLowerInvariant() + ":", StringComparison.InvariantCultureIgnoreCase));
            if (arg != null)
            {
                value = arg.Substring(arg.IndexOf(':') + 1);
                used = arg;
                return true;
            }

            return false;
        }

        private string GetFullParameterDescription(PropertyInfo property, IConsoleCommand command)
        {
            var name = Name ?? property.Name;

            if (IsRequired == false)
                name = "Type [default] to use default value: \"" + property.GetValue(command) + "\"\n" + name;

            if (!string.IsNullOrEmpty(Description))
                name = Description + "\n" + name;
            else
            {
                dynamic displayAttribute = property.GetCustomAttributes().SingleOrDefault(a => a.GetType().Name == "DisplayAttribute");
                if (displayAttribute != null && !string.IsNullOrEmpty(displayAttribute.Description))
                    name = displayAttribute.Description + "\n" + name;
                else
                {
                    dynamic descriptionAttribute = property.GetCustomAttributes().SingleOrDefault(a => a.GetType().Name == "DescriptionAttribute");
                    if (descriptionAttribute != null)
                        name = descriptionAttribute.Description + "\n" + name;
                }
            }

            return name + ": ";
        }
    }
}