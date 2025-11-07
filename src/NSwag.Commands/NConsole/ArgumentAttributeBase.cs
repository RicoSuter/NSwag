using System;
using System.Globalization;
using System.Reflection;
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)

namespace NConsole
{
    /// <summary>The argument attribute base class.</summary>
#pragma warning disable CA1710
    public abstract class ArgumentAttributeBase : Attribute
    {
        /// <summary>Gets the argument value.</summary>
        /// <param name="consoleHost">The command line host.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="property">The property.</param>
        /// <param name="command"></param>
        /// <param name="input">The output from the previous command in the chain.</param>
        /// <returns>The value.</returns>
        public abstract object GetValue(IConsoleHost consoleHost, string[] args, PropertyInfo property, IConsoleCommand command, object input, out string used);

        /// <summary>Converts a string value to a specific type.</summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns>The value.</returns>
        protected static object ConvertToType(string value, Type type)
        {
            if (type == typeof(Int16))
                return Int16.Parse(value, CultureInfo.InvariantCulture);
            if (type == typeof(Int32))
                return Int32.Parse(value, CultureInfo.InvariantCulture);
            if (type == typeof(Int64))
                return Int64.Parse(value, CultureInfo.InvariantCulture);

            if (type == typeof(UInt16))
                return UInt16.Parse(value, CultureInfo.InvariantCulture);
            if (type == typeof(UInt32))
                return UInt32.Parse(value, CultureInfo.InvariantCulture);
            if (type == typeof(UInt64))
                return UInt64.Parse(value, CultureInfo.InvariantCulture);

            if (type == typeof(Decimal))
                return Decimal.Parse(value, CultureInfo.InvariantCulture);

            if (type == typeof(Boolean))
                return Boolean.Parse(value);

            if (type == typeof(DateTime))
                return DateTime.Parse(value, CultureInfo.InvariantCulture);

            if (type.GetTypeInfo().IsEnum)
                return Enum.Parse(type, value, true);

            if (type == typeof(string[]))
                return !string.IsNullOrEmpty(value) ? value.Split(',') : [];

            return value;
        }
    }
}