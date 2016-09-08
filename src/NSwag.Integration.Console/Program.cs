using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSwag.Integration.WebAPI;

namespace NSwag.Integration.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            try
            {
                var errors = 0;
                var client = new PersonsClient("http://localhost:13452");

                var persons = await client.GetAllAsync();
                if (persons.Count == 0)
                    errors++;

                System.Console.WriteLine("Errors: " + errors);
            }
            catch (Exception exception)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine(exception);
            }
            System.Console.ReadKey();
        }
    }
}
