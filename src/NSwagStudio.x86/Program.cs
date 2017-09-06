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
            var assembly = Assembly.LoadFile(Path.GetFullPath("NSwagStudio.exe"));
            assembly.GetType("NSwagStudio.App").GetMethod("Main").Invoke(null, new object[0]);
        }
    }
}
