using System;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;

namespace Anrepack.Cli
{

    [HelpOption]
    [Subcommand(
        typeof(Repack),
        typeof(DownloadAndroidSdk),
        typeof(DownloadApktool),
        typeof(GenerateDebugKeystore)
    )]
    public class Program
    {

        public static Version AppVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        public static string AppName
        {
            get { return Assembly.GetExecutingAssembly().GetName().Name; }
        }

        static void Main(string[] args)
        {
            Console.WriteLine($"{AppName} {AppVersion}");
            CommandLineApplication.Execute<Program>(args);
        }

        void OnExecute()
        {
            Console.WriteLine("Usages can show with `--help` option.");
        }

    }

}
