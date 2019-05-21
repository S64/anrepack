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
            try
            {
                CommandLineApplication.Execute<Program>(args);
            }
            catch (AnrepackException e)
            {
                Console.Error.WriteLine("Error occured:");
                Console.Error.WriteLine(e.Message);
                Environment.Exit(1);
            }
        }

        void OnExecute()
        {
            Console.WriteLine("Usages can show with `--help` option.");
        }

    }

}
