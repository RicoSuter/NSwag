using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)

namespace NConsole
{
    /// <summary>An provided argument is not used.</summary>
    public class UnusedArgumentException : Exception
    {
        internal UnusedArgumentException(string arg)
#pragma warning disable CA1305
            : base(string.Format($"Unrecognised arguments are present: [{arg}]"))
        { }
    }

    /// <summary>A command base command line processor.</summary>
    public class CommandLineProcessor
    {
        private readonly IConsoleHost _consoleHost;
        private readonly Dictionary<string, Type> _commands = new Dictionary<string, Type>();
        private readonly IDependencyResolver _dependencyResolver;

        /// <summary>Initializes a new instance of the <see cref="CommandLineProcessor" /> class.</summary>
        /// <param name="consoleHost">The command line host.</param>
        /// <param name="dependencyResolver">The dependency resolver.</param>
        public CommandLineProcessor(IConsoleHost consoleHost, IDependencyResolver dependencyResolver = null, bool registerDefaultHelpCommand = true)
        {
            _consoleHost = consoleHost;
            _dependencyResolver = dependencyResolver;

            if (registerDefaultHelpCommand)
            {
                RegisterCommand<HelpCommand>("help");
            }
        }

        /// <summary>Gets the list of registered commands.</summary>
        public IReadOnlyDictionary<string, Type> Commands => _commands;

        /// <summary>Adds a command.</summary>
        /// <typeparam name="TCommandLineCommand">The type of the command.</typeparam>
        /// <param name="name">The name of the command.</param>
        public void RegisterCommand<TCommandLineCommand>(string name)
            where TCommandLineCommand : IConsoleCommand
        {
            RegisterCommand(name, typeof(TCommandLineCommand));
        }

        /// <summary>Adds a command.</summary>
        /// <typeparam name="TCommandLineCommand">The type of the command.</typeparam>
        public void RegisterCommand<TCommandLineCommand>()
            where TCommandLineCommand : IConsoleCommand
        {
            RegisterCommand(typeof(TCommandLineCommand));
        }

        /// <summary>Loads all commands from an assembly (command classes must have the CommandAttribute with a defined Name).</summary>
        /// <param name="assembly">The assembly.</param>
        public void RegisterCommandsFromAssembly(Assembly assembly)
        {
            var commandTypes = assembly.ExportedTypes.ToDictionary(t => t, t => t.GetTypeInfo().GetCustomAttribute<CommandAttribute>());
            foreach (var pair in commandTypes.Where(p => !string.IsNullOrEmpty(p.Value?.Name) && p.Key.GetTypeInfo().IsClass && !p.Key.GetTypeInfo().IsAbstract))
                RegisterCommand(pair.Value.Name, pair.Key);
        }

        /// <summary>Adds a command.</summary>
        /// <param name="commandType">Type of the command.</param>
        /// <exception cref="InvalidOperationException">The command has already been added.</exception>
        /// <exception cref="InvalidOperationException">The command class is missing the CommandAttribute attribute.</exception>
        public void RegisterCommand(Type commandType)
        {
            var commandAttribute = commandType.GetTypeInfo().GetCustomAttribute<CommandAttribute>();
            if (commandAttribute == null)
                throw new InvalidOperationException("The command class is missing the CommandAttribute attribute.");

            RegisterCommand(commandAttribute.Name, commandType);
        }

        /// <summary>Adds a command.</summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <exception cref="InvalidOperationException">The command has already been added.</exception>
        public void RegisterCommand(string name, Type commandType)
        {
            if (_commands.ContainsKey(name))
                throw new InvalidOperationException("The command '" + name + "' has already been added.");

            _commands.Add(name.ToLowerInvariant(), commandType);
        }

        /// <summary>Processes the command in the given command line arguments.</summary>
        /// <param name="args">The arguments.</param>
        /// <param name="input">The input for the first command.</param>
        /// <returns>The executed command.</returns>
        /// <exception cref="InvalidOperationException">The command could not be found.</exception>
        /// <exception cref="InvalidOperationException">No dependency resolver available to create a command without default constructor.</exception>
        public async Task<IList<CommandResult>> ProcessAsync(string[] args, object input = null)
        {
            var results = new List<CommandResult>();

            var commands = new List<string[]>();
            var commandArgs = new List<string>();
            foreach (var arg in args)
            {
                if (arg == "=")
                {
                    commands.Add(commandArgs.ToArray());
                    commandArgs = new List<string>();
                }
                else
                    commandArgs.Add(arg);
            }
            commands.Add(commandArgs.ToArray());

            foreach (var command in commands)
            {
                var result = await ProcessSingleAsync(command, results.LastOrDefault()?.Output);
                results.Add(result);
            }

            return results;
        }

        /// <summary>
        /// Search for command type matching a command name.
        /// </summary>
        /// <param name="commandName">Name of the command to search for.</param>
        /// <returns>The matching command, otherwise NULL.</returns>
        private Type TryLookupCommandType(string commandName)
        {
            commandName = commandName.ToLowerInvariant();
            Type commandType = null;
            _commands.TryGetValue(commandName, out commandType);
            return commandType;
        }

        /// <summary>Processes the command in the given command line arguments.</summary>
        /// <param name="args">The arguments.</param>
        /// <param name="input">The input for the command.</param>
        /// <returns>The executed command.</returns>
        /// <exception cref="InvalidOperationException">The command could not be found.</exception>
        /// <exception cref="InvalidOperationException">No dependency resolver available to create a command without default constructor.</exception>
        public async Task<CommandResult> ProcessSingleAsync(string[] args, object input = null)
        {
            var usedArgs = new List<string>();
            GetCommandNameAndArguments(args, out string commandName, out IEnumerable<string> commandArguments);

            var commandType = TryLookupCommandType(commandName);
            if (commandType != null)
            {
                var command = CreateCommand(commandType);

                foreach (var property in commandType.GetRuntimeProperties())
                {
                    string usedArg = null;
                    var argumentAttribute = property.GetCustomAttribute<ArgumentAttributeBase>();
                    if (argumentAttribute != null)
                    {
                        var value = argumentAttribute.GetValue(_consoleHost, args, property, command, input, out usedArg);
                        if (value != null)
                            property.SetValue(command, value);
                        if (usedArg != null)
                            usedArgs.Add(usedArg);
                    }
                }

                if (usedArgs.Count != commandArguments.Count())
                {
                    var unusedArgs = new List<string>();
                    foreach (string arg in commandArguments)
                    {
                        if (!usedArgs.Contains(arg))
                        {
                            unusedArgs.Add(arg);
                        }
                    }

                    throw new UnusedArgumentException($"Used arguments ({usedArgs.Count}) != Provided arguments ({commandArguments.Count()}) -> Check [{string.Join(", ", unusedArgs)}]");
                }

                var output = await command.RunAsync(this, _consoleHost);
                return new CommandResult
                {
                    Command = command,
                    Output = output
                };
            }
            else
                throw new InvalidOperationException("The command '" + commandName + "' could not be found.");
        }

        /// <summary>Processes the command in the given command line arguments.</summary>
        /// <param name="args">The arguments.</param>
        /// <param name="input">The output from the previous command.</param>
        /// <returns>The exeucuted command.</returns>
        /// <exception cref="InvalidOperationException">The command could not be found.</exception>
        /// <exception cref="InvalidOperationException">No dependency resolver available to create a command without default constructor.</exception>
        public IList<CommandResult> Process(string[] args, object input = null)
        {
            return ProcessAsync(args, input).GetAwaiter().GetResult();
        }

        /// <summary>Processes the command in the given command line arguments.</summary>
        /// <param name="args">The arguments.</param>
        /// <param name="input">The output from the previous command.</param>
        /// <returns>The exeucuted command.</returns>
        public IList<CommandResult> ProcessWithExceptionHandling(string[] args, object input = null)
        {
            try
            {
                return ProcessAsync(args, input).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                _consoleHost.WriteError(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Read the command name using console host if it was not provided by call.
        /// </summary>
        /// <returns>Command name input by user</returns>
        private string ReadCommandNameInteractive()
        {
            _consoleHost.WriteMessage("Commands: \n");
            foreach (var command in Commands)
                _consoleHost.WriteMessage("  " + command.Key + "\n");

            return _consoleHost.ReadValue("Command: ");
        }

        /// <summary>Gets the name of the command to execute.</summary>
        /// <param name="args">The arguments.</param>
        protected void GetCommandNameAndArguments(string[] args, out string commandName, out IEnumerable<string> commandArguments)
        {
            commandName = string.Empty;
            commandArguments = new List<string>();

            bool hasArguments = (args.Length > 0) && (args[0].Length > 0) && (char.IsLetter(args[0][0]));
            if (hasArguments)
            {
                commandName = args[0];
                commandArguments = args.Skip(1);
            }
            else if (_consoleHost.InteractiveMode)
            {
                commandName = ReadCommandNameInteractive();
                commandArguments = args;
            }
            else
            {
                throw new InvalidOperationException($"Could not retrieve command from arguments {string.Join(", ", args)}");
            }
        }

        /// <exception cref="InvalidOperationException">No dependency resolver available to create a command without default constructor.</exception>
        private IConsoleCommand CreateCommand(Type commandType)
        {
            var constructors = commandType.GetTypeInfo().DeclaredConstructors;
            IConsoleCommand command;

            if (constructors.Any())
            {
                var constructor = constructors.First(c => !c.IsStatic);

                if (constructor.GetParameters().Length > 0 && _dependencyResolver == null)
                    throw new InvalidOperationException("No dependency resolver available to create a command without default constructor.");

                var parameters = constructor.GetParameters()
                    .Select(param => _dependencyResolver.GetService(param.ParameterType))
                    .ToArray();

                command = (IConsoleCommand)constructor.Invoke(parameters);
            }
            else
            {
                if (_dependencyResolver == null)
                {
                    throw new InvalidOperationException($"Cannot create an instance of {commandType} because it does not " +
                                                        $"have any accessible constructors and no dependency resolver is available.");
                }

                command = (IConsoleCommand)_dependencyResolver.GetService(commandType);
            }

            return command;
        }
    }
}
