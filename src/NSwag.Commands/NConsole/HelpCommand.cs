using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)

namespace NConsole
{
    /// <summary>The help command to show the availble list of commands.</summary>
    public class HelpCommand : IConsoleCommand
    {
        /// <summary>Gets the command to print infos for (by default not set => prints all commands).</summary>
        [Argument(Position = 1, IsRequired = false, ShowPrompt = false)]
        public string Command { get; set; }

        /// <summary>
        /// Usage line of help command.
        /// </summary>
        public string Usage { get; set; } = "  myapp.exe myCommand /myParameter:myValue /mySecondParameter:myValue\n\n";

        /// <summary>Runs the command.</summary>
        /// <param name="processor">The processor.</param>
        /// <param name="host">The host.</param>
        /// <returns>The input object for the next command.</returns>
        public Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            if (!string.IsNullOrEmpty(Command) && char.IsLetter(Command[0]))
            {
                if (processor.Commands.ContainsKey(Command))
                    HelpCommand.PrintCommand(host, processor.Commands.Single(c => c.Key == Command));
                else
                    host.WriteMessage("Command '" + Command + "' could not be found...");
            }
            else
            {
                host.WriteMessage("\n");
                host.WriteMessage("Usage:\n\n");
                host.WriteMessage(Usage);
                host.WriteMessage("Commands:\n\n");
                foreach (var command in processor.Commands.Where(c => c.Key != "help"))
                    host.WriteMessage("  " + command.Key + "\n");

                PromptInteractiveOnly(host, "Press <enter> key to show commands...");

                foreach (var command in processor.Commands)
                {
                    if (command.Key != "help")
                    {
                        HelpCommand.PrintCommand(host, command);
                        PromptInteractiveOnly(host, "Press <enter> key for next command...");
                    }
                }
            }

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Prompt user to press a key before continuing
        /// </summary>
        /// <param name="message">Message to display</param>
        private static void PromptInteractiveOnly(IConsoleHost host, string message)
        {
            if (host.InteractiveMode)
            {
                host.ReadValue(message);
            }
        }

        private static void PrintCommand(IConsoleHost host, KeyValuePair<string, Type> pair)
        {
            var commandType = pair.Value;

            host.WriteMessage("\nCommand: ");
            host.WriteMessage(pair.Key + "\n");

            var commandAttribute = commandType.GetTypeInfo().GetCustomAttribute<CommandAttribute>();
            if (commandAttribute != null && !string.IsNullOrEmpty(commandAttribute.Description))
                host.WriteMessage("  " + commandAttribute.Description + "\n");
            else
            {
                dynamic descriptionAttribute = commandType.GetTypeInfo().GetCustomAttributes().SingleOrDefault(a => a.GetType().Name == "DescriptionAttribute");
                if (descriptionAttribute != null)
                    host.WriteMessage("  " + descriptionAttribute.Description + "\n");
            }

            host.WriteMessage("\nArguments: \n");
            foreach (var property in commandType.GetRuntimeProperties())
            {
                var argumentAttribute = property.GetCustomAttribute<ArgumentAttribute>();
                if (argumentAttribute != null && !string.IsNullOrEmpty(argumentAttribute.Name))
                {
                    if (argumentAttribute.Position > 0)
                        host.WriteMessage("  Argument Position " + argumentAttribute.Position + "\n");
                    else
                        host.WriteMessage("  " + argumentAttribute.Name + "\n");

                    if (!string.IsNullOrEmpty(argumentAttribute.Description))
                        host.WriteMessage("    " + argumentAttribute.Description + "\n");

                    dynamic parameterDescriptionAttribute = property.GetCustomAttributes().SingleOrDefault(a => a.GetType().Name == "DescriptionAttribute");
                    if (parameterDescriptionAttribute != null)
                        host.WriteMessage("    " + parameterDescriptionAttribute.Description + "\n");
                }
            }
        }
    }
}
