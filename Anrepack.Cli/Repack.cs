using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;

namespace Anrepack.Cli
{

    [HelpOption]
    [Command(
        SUBCOMMAND_NAME,
        ThrowOnUnexpectedArgument = false,
        Description = "Execute repackage operation.",
        ExtendedHelpText = "*If not-defined argument present, Passes as Python script's argument."
    )]
    public class Repack : IAnrepackCommand
    {

        public const string SUBCOMMAND_NAME = "repack";

        private static readonly char DSC = AndroidCore.DSC;

        public const string JAVA_HOME_ARG = "--java";
        public const string ANDROID_HOME_ARG = "--android";
        public const string APKTOOL_ARG = "--jar";
        public const string KEYSTORE_ARG = "--keystore";
        public const string TMPDIR_ARG = "--tmpdir";
        public const string OUTPUT_ARG = "--output";

        private const string TARGET_APK_ARG = "--apk";
        private const string KEYSTORE_PASSWORD_ARG = "--keystore-password";
        private const string KEYSTORE_ALIAS_ARG = "--keystore-alias";
        private const string PYTHON_SCRIPT_ARG = "--script";

        [Option(
            JAVA_HOME_ARG,
            CommandOptionType.SingleOrNoValue,
            Description = "Optional. Set custom `JAVA_HOME` location."
        )]
        string JavaHomePathArg { get; }

        [Option(
            ANDROID_HOME_ARG,
            CommandOptionType.SingleOrNoValue,
            Description = "Optional. Set custom `ANDROID_HOME` location."
        )]
        string AndroidHomePathArg { get; }

        [Option(
            APKTOOL_ARG,
            CommandOptionType.SingleOrNoValue,
            Description = "Optional. Set custom `apktool.jar` location."
        )]
        string ApktoolJarPathArg { get; }

        [Option(
            KEYSTORE_ARG,
            CommandOptionType.SingleOrNoValue,
            Description = "Optional. Set signing keystore location. Default is `debug.keystore`."
        )]
        string KeyStorePathArg { get; }

        [Option(
            TMPDIR_ARG,
            CommandOptionType.SingleOrNoValue,
            Description = "Optional. Set temporary directory path."
        )]
        string TmpPathArg { get; }

        [Required(AllowEmptyStrings = false)]
        [Option(
            TARGET_APK_ARG,
            CommandOptionType.SingleValue,
            Description = "Path of target apk to decode."
        )]
        string TargetApkArg { get; }

        [Option(
            KEYSTORE_PASSWORD_ARG,
            CommandOptionType.SingleOrNoValue,
            Description = "Signing keystore password."
        )]
        string KeyStorePassword { get; set; }

        [Option(
            KEYSTORE_ALIAS_ARG,
            CommandOptionType.SingleOrNoValue,
            Description = "Signing keystore alias."
        )]
        string KeyStoreAlias { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Option(
            PYTHON_SCRIPT_ARG,
            CommandOptionType.SingleValue,
            Description = "Path of operation script."
        )]
        string PythonScriptArg { get; }

        [Required(AllowEmptyStrings = false)]
        [Option(
            OUTPUT_ARG,
            CommandOptionType.SingleValue,
            Description = "Path of output apk."
        )]
        string OutputPathArg { get; }

        IReadOnlyList<string> RemainingArguments { get; }

        private DirectoryInfo JavaHome { get; set; }
        private DirectoryInfo AndroidHome { get; set; }
        private FileInfo Apktool { get; set; }
        private FileInfo KeyStore { get; set; }

        private FileInfo TargetApk { get; set; }
        private FileInfo OutputApk { get; set; }

        private DirectoryInfo TempWorkDir { get; set; }

        private FileInfo PythonScript { get; set; }

        public void OnExecute()
        {
            InitTempWorkDir();

            JavaHome = PathResolver.GetJavaHome(JavaHomePathArg);
            AndroidHome = PathResolver.GetAndroidHome(AndroidHomePathArg);
            Apktool = PathResolver.GetApktoolPath(ApktoolJarPathArg);
            KeyStore = PathResolver.GetKeyStorePath(KeyStorePathArg);

            {
                if (KeyStorePassword == null)
                {
                    Console.WriteLine($"Info: {KEYSTORE_PASSWORD_ARG} not set. Try to use 'android'.");
                    KeyStorePassword = "android";
                }
                if (KeyStoreAlias == null)
                {
                    Console.WriteLine($"Info: {KEYSTORE_ALIAS_ARG} not set. Try to use 'androiddebugkey'.");
                    KeyStoreAlias = "androiddebugkey";
                }
            }
            {
                TargetApk = new FileInfo(TargetApkArg);
                if (!TargetApk.Exists)
                {
                    throw new AnrepackException($"{TargetApkArg} ({TARGET_APK_ARG}) is not found.");
                }
            }
            {
                OutputApk = new FileInfo(OutputPathArg);
                if (OutputApk.Exists)
                {
                    throw new AnrepackException($"{OUTPUT_ARG} ({OutputPathArg}) already exists.");
                }
                if (!OutputApk.Extension.ToLower().Equals(".apk"))
                {
                    Console.WriteLine($"W: {OUTPUT_ARG} ({OutputPathArg})'s extension is not apk.");
                }
            }
            {
                PythonScript = new FileInfo(PythonScriptArg);
                if (!PythonScript.Exists)
                {
                    throw new AnrepackException($"{PythonScriptArg} ({PYTHON_SCRIPT_ARG}) is not found.");
                }
            }
            {
                Console.WriteLine("Decode apk using apktool...");

                (new ShellConnector(
                    GetJre(),
                    $"-jar \"{Apktool}\" d {TargetApk.FullName} --output=\"{GetDecodedDir().FullName}\""
                )).Execute();

                Console.WriteLine("Decoded.");
            }
            {
                Console.WriteLine("Run script...");
                new PythonScriptRunner(
                    PythonScript,
                    GetDecodedDir(),
                    RemainingArguments.ToArray()
                ).Execute();
                Console.WriteLine("Done.");
            }
            {
                Console.WriteLine("Re-Build apk using apktool...");

                (new ShellConnector(
                    GetJre(),
                    $"-jar \"{Apktool}\" b \"{GetDecodedDir().FullName}\" --output={GetRebuiltApk().FullName}"
                )).Execute();

                Console.WriteLine("Built.");
            }
            {
                Console.WriteLine("Signing...");

                var jarsignerArgs = $"-verbose -sigalg SHA1withRSA -digestalg SHA1 -keystore \"{KeyStore.FullName}\" -storepass \"{KeyStorePassword}\" -signedjar \"{TempWorkDir.FullName}{DSC}signed.apk\" \"{GetRebuiltApk().FullName}\" \"{KeyStoreAlias}\"";
                (new ShellConnector(
                    GetJarSigner(),
                    jarsignerArgs
                )).Execute();

                Console.WriteLine("Signed.");
            }
            {
                if (!OutputApk.Directory.Exists)
                {
                    OutputApk.Directory.Create();
                }
                File.Move(
                    GetRebuiltApk().FullName,
                    OutputApk.FullName
                );
                Console.WriteLine("Completed!");
            }
        }

        private void InitTempWorkDir()
        {
            var id = Guid.NewGuid();
            if (TmpPathArg == null)
            {
                TempWorkDir = new DirectoryInfo($"{Path.GetTempPath()}{DSC}{id}");
            }
            else
            {
                var parent = new DirectoryInfo(TmpPathArg);
                if (!parent.Exists)
                {
                    throw new AnrepackException($"Passed {TMPDIR_ARG} is invalid.");
                }
                TempWorkDir = new DirectoryInfo($"{parent.FullName}{DSC}{id}");
            }
            TempWorkDir.Create();

            Console.WriteLine($"Tempdir {TempWorkDir.FullName} Created.");
        }

        private FileInfo GetJre()
        {
            return new FileInfo(
                $"{JavaHome.FullName}{DSC}bin{DSC}java"
            );
        }

        private FileInfo GetJarSigner()
        {
            return new FileInfo(
                $"{JavaHome.FullName}{DSC}bin{DSC}jarsigner"
            );
        }

        private DirectoryInfo GetDecodedDir()
        {
            var dir = new DirectoryInfo(
                $"{TempWorkDir.FullName}{DSC}apktool-output"
            );
            /*
            if (dir.Exists)
            {
                throw new AnrepackException("Apktool output dir already exists.");
            }
            */
            return dir;
        }

        private FileInfo GetRebuiltApk()
        {
            var file = new FileInfo(
                $"{TempWorkDir.FullName}{DSC}rebuilt-unsigned.apk"
            );
            /*
            if (file.Exists)
            {
                throw new AnrepackException("Built apk already exists.");
            }
            */
            return file;
        }

    }

}
