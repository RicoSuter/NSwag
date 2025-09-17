using System.Reflection;
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)

namespace NConsole
{
    /// <summary>Attribute to define a switch/boolean attribute.</summary>
    public class SwitchAttribute : ArgumentAttributeBase
    {
        /// <summary>Gets or sets the short name without the '-' prefix.</summary>
        public string ShortName { get; set; }

        /// <summary>Gets or sets the long name without the '--' prefix.</summary>
        public string LongName { get; set; }

        /// <summary>Gets the argument value.</summary>
        /// <param name="consoleHost">The command line host.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="property">The property.</param>
        /// <param name="command">The command.</param>
        /// <param name="input">The output from the previous command in the chain.</param>
        /// <returns>The value.</returns>
        public override object GetValue(IConsoleHost consoleHost, string[] args, PropertyInfo property, IConsoleCommand command, object input, out string used)
        {
            foreach (var argument in args)
            {
                used = argument;
                if (argument == "-" + ShortName)
                    return true;

                if (argument == "--" + LongName)
                    return true;

                if (argument == "/" + LongName)
                    return true;
            }
            used = null;
            return false; 
        }
    }
}
