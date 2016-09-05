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

            var name = await client.GetNameAsync(123);
            if (name != "Foo bar: 123")
                errors++; 

            System.Console.WriteLine("Errors: " + errors);
            System.Console.ReadKey();
        }
    }
}
