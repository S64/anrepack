using System;
using System.Net;
using McMaster.Extensions.CommandLineUtils;

namespace Anrepack.Cli
{

    [Command(SUBCOMMAND_NAME)]
    public class DownloadApktool : IAnrepackCommand
    {

        public const string SUBCOMMAND_NAME = "install-apktool";

        public void OnExecute()
        {
            var downloader = new ApktoolDownloader();

            Console.WriteLine($"Download apktool to `{ApktoolDownloader.TempJarDest.FullName}`...");
            try
            {
                downloader.Exec().Wait();
            }
            catch (ADException e)
            {
                throw new AnrepackException(e.Message);
            }

            Console.WriteLine("Apktool downloaded.");
        }

    }

}
