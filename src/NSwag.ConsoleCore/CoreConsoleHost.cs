using System;
using NConsole;

namespace NSwag
{
    // TODO: Fix ConsoleHost so that it also works in .NET Core

    public class CoreConsoleHost : IConsoleHost
    {
        public void WriteMessage(string message)
        {
            Console.Write(message);
        }

        public void WriteError(string message)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(message);
            Console.ForegroundColor = color;
        }

        public string ReadValue(string message)
        {
            Console.Write(message);
            return Console.ReadLine();
        }
    }
}