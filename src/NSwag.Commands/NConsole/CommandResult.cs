namespace NConsole
{
    /// <summary>A command result.</summary>
    public class CommandResult
    {
        /// <summary>Gets or sets the command.</summary>
        public IConsoleCommand Command { get; set; }

        /// <summary>Gets or sets the command output.</summary>
        public object Output { get; set; }
    }
}