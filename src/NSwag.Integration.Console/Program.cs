using System;
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
            var errors = 0;
            var client = new PersonsClient();

            var name = await client.GetNameAsync(Guid.Empty);
            if (name != "Foo bar: " + Guid.Empty)
                errors++;

            System.Console.WriteLine("Errors: " + errors);
            System.Console.ReadKey();
        }
    }
}
