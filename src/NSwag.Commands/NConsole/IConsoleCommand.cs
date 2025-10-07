using System.Threading.Tasks;

namespace NConsole
{
    /// <summary>A command line command.</summary>
    public interface IConsoleCommand
    {
        /// <summary>Runs the command.</summary>
        /// <param name="processor">The processor.</param>
        /// <param name="host">The host.</param>
        /// <returns>The output.</returns>
        Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host);
    }
}