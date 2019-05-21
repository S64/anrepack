using System;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using Sentry;
using System.Linq;
using System.IO;

namespace Anrepack.Cli
{

    [HelpOption]
    [Subcommand(
        typeof(AnrepackVersion),
        typeof(Repack),
        typeof(DownloadAndroidSdk),
        typeof(DownloadApktool),
        typeof(GenerateDebugKeystore)
    )]
    public class Program
    {

        private const string SentryDsnResourceName = "Anrepack.Cli.Resources.SentryDsn.txt";

        private static AssemblyInformationalVersionAttribute InfoAttr
        {
            get { return Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>(); }
        }

        public static string AppVersion
        {
            get { return InfoAttr.InformationalVersion; }
        }

        public static string AppName
        {
            get { return Assembly.GetExecutingAssembly().GetName().Name; }
        }

        static void Main(string[] args)
        {
            string sentryDsn = LoadSentryDsn();
            if (sentryDsn != null)
            {
                using (SentrySdk.Init(sentryDsn))
                {
                    StartApp(args);
                }
            }
            else
            {
                StartApp(args);
            }
        }

        private static void StartApp(string[] args)
        {
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

        private static string LoadSentryDsn()
        {
            if (Assembly.GetExecutingAssembly().GetManifestResourceNames().Contains(SentryDsnResourceName))
            {
                string read;
                using (var src = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(SentryDsnResourceName)))
                {
                    read = src.ReadToEnd();
                }
                return read.Trim();
            }

            Console.WriteLine("NOTE: Sentry resource not found.");
            return null;
        }

    }

}
