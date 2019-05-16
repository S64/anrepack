using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;

namespace Anrepack.Cli
{

    [Command(SUBCOMMAND_NAME, ThrowOnUnexpectedArgument = false)]
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

        [Option(JAVA_HOME_ARG, CommandOptionType.SingleOrNoValue)]
        string JavaHomePathArg { get; }

        [Option(ANDROID_HOME_ARG, CommandOptionType.SingleOrNoValue)]
        string AndroidHomePathArg { get; }

        [Option(APKTOOL_ARG, CommandOptionType.SingleOrNoValue)]
        string ApktoolJarPathArg { get; }

        [Option(KEYSTORE_ARG, CommandOptionType.SingleOrNoValue)]
        string KeyStorePathArg { get; }

        [Option(TMPDIR_ARG, CommandOptionType.SingleOrNoValue)]
        string TmpPathArg { get; }

        [Required]
        [Option(TARGET_APK_ARG, CommandOptionType.SingleValue)]
        string TargetApkArg { get; }

        [Option(KEYSTORE_PASSWORD_ARG, CommandOptionType.SingleOrNoValue)]
        string KeyStorePassword { get; set; }

        [Option(KEYSTORE_ALIAS_ARG, CommandOptionType.SingleOrNoValue)]
        string KeyStoreAlias { get; set; }

        [Required]
        [Option(PYTHON_SCRIPT_ARG, CommandOptionType.SingleValue)]
        string PythonScriptArg { get; }

        [Required]
        [Option(OUTPUT_ARG, CommandOptionType.SingleValue)]
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
                    throw new FileNotFoundException($"{TargetApkArg} ({TARGET_APK_ARG}) is not found.");
                }
            }
            {
                OutputApk = new FileInfo(OutputPathArg);
                if (OutputApk.Exists)
                {
                    throw new InvalidOperationException($"{OUTPUT_ARG} ({OutputPathArg}) already exists.");
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
                    throw new FileNotFoundException($"{PythonScriptArg} ({PYTHON_SCRIPT_ARG}) is not found.");
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
                    throw new ArgumentException($"Passed {TMPDIR_ARG} is invalid.");
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
                throw new InvalidOperationException("Apktool output dir already exists.");
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
                throw new InvalidOperationException("Built apk already exists.");
            }
            */
            return file;
        }

    }

}
