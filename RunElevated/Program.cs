using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace RunElevated
{
    class Program
    {
        static void Main(string[] args)
        {
            WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            bool hasAdministrativeRight = principal.IsInRole(WindowsBuiltInRole.Administrator);

            Console.WriteLine(hasAdministrativeRight);

            Console.ReadKey();
        }
    }

    public class Runner
    {
        public void Run()
        {
            var asmPath = typeof(Program).Assembly;
            var uri = new Uri(asmPath.CodeBase);
            var file = new FileInfo(uri.LocalPath);

            var process = new ProcessStartInfo
            {
                UseShellExecute = true,
                WorkingDirectory = file.DirectoryName ?? Environment.CurrentDirectory,
                FileName = file.FullName,
                Verb = "runas"
            };

            try
            {
                var executed = Process.Start(process);
                executed.WaitForExit();

                Console.WriteLine("Successfully elevated!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to elevate." + ex.Message);
            }
        }
    }

    [TestFixture]
    [Explicit]
    // ReSharper disable once InconsistentNaming
    internal class Runner_Should
    {
        [Test]
        public void Do()
        {
            new Runner().Run();
        }
    }
}
