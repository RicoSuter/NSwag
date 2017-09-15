using System;
using System.IO;
using System.Reflection;

namespace NSwagStudio.x86
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            RegisterAssemblyLoader();

            var assembly = Assembly.LoadFile(Path.GetFullPath("NSwagStudio.exe"));
            assembly.GetType("NSwagStudio.App").GetMethod("Main").Invoke(null, new object[0]);
        }

        private static void RegisterAssemblyLoader()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
            {
                var index = eventArgs.Name.IndexOf(",", StringComparison.InvariantCulture);
                if (index > 0)
                {
                    var name = eventArgs.Name.Substring(0, index);
                    if (File.Exists(name + ".dll"))
                        return Assembly.LoadFile(Path.GetFullPath(name + ".dll"));
                }

                return null;
            };
        }
    }
}
