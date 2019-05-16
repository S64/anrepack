using System;
using System.IO;

namespace Anrepack.Cli
{
    public class PathResolver
    {

        private static readonly char DSC = AndroidCore.DSC;

        private static readonly DirectoryInfo HomeDir = AndroidCore.HomeDir;

        public static DirectoryInfo GetJavaHome(string javaHomeArg = null)
        {
            if (javaHomeArg == null)
            {
                var javaDir = JavaResolver.ResolveJavaDir();
                if (javaDir == null)
                {
                    throw new InvalidOperationException("`java` executable not found.");
                }
                return javaDir;
            }
            else
            {
                var javaDir = new DirectoryInfo(javaHomeArg);
                if (!javaDir.Exists)
                {
                    throw new ArgumentException($"Passed {Repack.JAVA_HOME_ARG} is invalid.");
                }
                return javaDir;
            }
        }

        public static DirectoryInfo GetAndroidHome(string androidHomeArg = null)
        {
            if (androidHomeArg == null)
            {
                var dir = AndroidResolver.ResolveAndroidDir();
                if (dir == null)
                {
                    throw new InvalidOperationException("`adb` executable not found.");
                }
                return dir;
            }
            else
            {
                var dir = new DirectoryInfo(androidHomeArg);
                if (!dir.Exists)
                {
                    throw new ArgumentException($"Passed {Repack.ANDROID_HOME_ARG} is invalid.");
                }
                return dir;
            }
        }

        public static FileInfo GetApktoolPath(string apktoolJarPathArg)
        {
            if (apktoolJarPathArg == null)
            {
                if (ApktoolDownloader.TempJarDest.Exists)
                {
                    Console.WriteLine($"Info: Apktool detected at `{ApktoolDownloader.TempJarDest}`.");
                    return ApktoolDownloader.TempJarDest;
                }
                else
                {
                    var file = Which.Find("apktool.jar");
                    if (file == null)
                    {
                        throw new InvalidOperationException($"`apktool.jar` executable not found.");
                    }
                    return file;
                }
            }
            else
            {
                var file = new FileInfo(apktoolJarPathArg);
                if (!file.Exists)
                {
                    throw new ArgumentException($"Passed {Repack.APKTOOL_ARG} is invalid.");
                }
                return file;
            }
        }

        public static FileInfo GetKeyStorePath(string keyStorePathArg = null)
        {
            if (keyStorePathArg == null)
            {
                var file = AndroidResolver.GetDebugKeyStore();
                if (!file.Exists)
                {
                    throw new InvalidOperationException($"`debug.keystore` not found.");
                }
                return file;
            }
            else
            {
                var file = new FileInfo(keyStorePathArg);
                if (!file.Exists)
                {
                    throw new ArgumentException($"Passed {Repack.KEYSTORE_ARG} is invalid.");
                }
                return file;
            }
        }

    }
}
