﻿using System;
using McMaster.Extensions.CommandLineUtils;

namespace Anrepack.Cli
{

    [Command(
        SUBCOMMAND_NAME,
        Description = "Download Android SDK to default location."
    )]
    public class DownloadAndroidSdk : IAnrepackCommand
    {

        public const string SUBCOMMAND_NAME = "install-android";

        public void OnExecute()
        {
            var downloader = new AndroidSdkDownloader();

            Console.WriteLine($"Output to: {downloader.TempDir.FullName}");

            Console.WriteLine("Download android sdk...");
            try
            {
                downloader.DownloadSdkToolsAsync().Wait();
            }
            catch (ASDException e)
            {
                throw new AnrepackException(e.Message);
            }

            Console.WriteLine("Unarchive android sdk...");
            downloader.UnarchiveSdkTools();

            Console.WriteLine($"Move SDKs to: {downloader.GetDefaultDirectory()}");
            try
            {
                downloader.MoveUnarchivedSdkToolsToDefaultDir();
            }
            catch (ASDException e)
            {
                throw new AnrepackException(e.Message);
            }

            Console.WriteLine("Android SDK Tools installed. Please execute following command to install adb:");
            Console.WriteLine(); // empty line
            Console.WriteLine(
                $"\t{downloader.GetSdkManagerExecutable()} \"platform-tools\" --sdk_root=\"{downloader.GetDefaultDirectory()}\""
            );
            Console.WriteLine(); // empty line

            if (JavaResolver.ResolveJavaDir() == null)
            {
                Console.WriteLine(); // empty line
                Console.WriteLine("NOTE: JDK installation not found. If you want to install it, We suggest using SDKMAN.");
                Console.WriteLine(); // empty line
                Console.WriteLine(
                    "\tSee: https://sdkman.io/install"
                );
                Console.WriteLine(); // empty line
            }
        }
    }

}
