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
                    throw new AnrepackException("`java` executable not found.");
                }
                return javaDir;
            }
            else
            {
                var javaDir = new DirectoryInfo(javaHomeArg);
                if (!javaDir.Exists)
                {
                    throw new AnrepackException($"Passed {Repack.JAVA_HOME_ARG} is invalid.");
                }
                return javaDir;
            }
        }

        public static DirectoryInfo GetAndroidHome(string androidHomeArg = null)
        {
            if (androidHomeArg == null)
            {
                DirectoryInfo dir;
                try
                {
                    dir = AndroidResolver.ResolveAndroidDir();
                }
                catch (ARException e)
                {
                    throw new AnrepackException(e.Message);
                }
                if (dir == null)
                {
                    throw new AnrepackException("`adb` executable not found.");
                }
                return dir;
            }
            else
            {
                var dir = new DirectoryInfo(androidHomeArg);
                if (!dir.Exists)
                {
                    throw new AnrepackException($"Passed {Repack.ANDROID_HOME_ARG} is invalid.");
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
                        throw new AnrepackException($"`apktool.jar` executable not found.");
                    }
                    return file;
                }
            }
            else
            {
                var file = new FileInfo(apktoolJarPathArg);
                if (!file.Exists)
                {
                    throw new AnrepackException($"Passed {Repack.APKTOOL_ARG} is invalid.");
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
                    throw new AnrepackException($"`debug.keystore` not found.");
                }
                return file;
            }
            else
            {
                var file = new FileInfo(keyStorePathArg);
                if (!file.Exists)
                {
                    throw new AnrepackException($"Passed {Repack.KEYSTORE_ARG} is invalid.");
                }
                return file;
            }
        }

    }
}
