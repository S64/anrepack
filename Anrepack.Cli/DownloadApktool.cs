﻿using System;
using System.Net;
using McMaster.Extensions.CommandLineUtils;

namespace Anrepack.Cli
{

    [Command(
        SUBCOMMAND_NAME,
        Description = "Download Apktool to Anrepack's temporary location."
    )]
    public class DownloadApktool : IAnrepackCommand
    {

        public const string SUBCOMMAND_NAME = "install-apktool";

        public void OnExecute()
        {
            var downloader = new ApktoolDownloader();

            Console.WriteLine($"Download Apktool to `{ApktoolDownloader.TempJarDest.FullName}`...");
            downloader.Exec().Wait();

            Console.WriteLine("Apktool downloaded.");
        }

    }

}
