using System;
using System.Reflection;

namespace NConsole
{
    /// <summary>A command line host implementation which uses System.Console.</summary>
    public class ConsoleHost : IConsoleHost
    {
        private readonly Type _consoleType = typeof(Console);
        private readonly MethodInfo _consoleWriteMethod;
        private readonly MethodInfo _consoleReadLineMethod;
        private readonly PropertyInfo _consoleForegroundColorProperty;

        /// <summary>Initializes a new instance of the <see cref="ConsoleHost"/> class.</summary>
        public ConsoleHost()
        {
            _consoleWriteMethod = _consoleType.GetRuntimeMethod("Write", new[] { typeof(string) });
            _consoleReadLineMethod = _consoleType.GetRuntimeMethod("ReadLine", []);
            _consoleForegroundColorProperty = _consoleType.GetRuntimeProperty("ForegroundColor");
        }

        /// <summary>Initializes a new instance of the <see cref="ConsoleHost"/> class.</summary>
        /// <param name="interactiveMode">Specifies whether interactive mode is enabled.</param>
        public ConsoleHost(bool interactiveMode)
            : this()
        {
            InteractiveMode = interactiveMode;
        }

        /// <summary>Gets or sets a value indicating whether interactive mode is enabled (i.e. ReadValue() is allowed).</summary>
        public bool InteractiveMode { get; set; } = true;

        /// <summary>Writes a message to the console.</summary>
        /// <param name="message">The message.</param>
        public void WriteMessage(string message)
        {
            _consoleWriteMethod.Invoke(null, new object[] { message });
        }

        /// <summary>Writes an error message to the console.</summary>
        /// <param name="message">The message.</param>
        public void WriteError(string message)
        {
            var color = _consoleForegroundColorProperty.GetValue(null);
            _consoleForegroundColorProperty.SetValue(null, 12); // red
            _consoleWriteMethod.Invoke(null, new object[] { message });
            _consoleForegroundColorProperty.SetValue(null, color);
        }

        /// <summary>Reads a value from the console.</summary>
        /// <param name="message">The message.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">Cannot read value from command line because interactive mode is disabled.</exception>
        public string ReadValue(string message)
        {
            if (!InteractiveMode)
                throw new InvalidOperationException("Cannot read value from command line because interactive mode is disabled.");

            _consoleWriteMethod.Invoke(null, new object[] { "\n" + message });
            return (string)_consoleReadLineMethod.Invoke(null, []);
        }
    }
}