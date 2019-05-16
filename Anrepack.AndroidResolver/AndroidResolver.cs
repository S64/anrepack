using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Anrepack
{
    public class AndroidResolver
    {

        public static DirectoryInfo ResolveAndroidDir()
        {
            {
                var env = Environment.GetEnvironmentVariable("ANDROID_HOME");
                if (env != null)
                {
                    return new DirectoryInfo(env);
                }
            }
            {
                var adb = Which.Find("adb");
                if (adb != null)
                {
                    return adb.Directory.Parent; // ./../
                }
            }
            {
                string[] dirs;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    dirs = new string[] {
                        AndroidCore.UserAndroidHomePath_OSX,
                        AndroidCore.SystemAndroidHomePath_OSX
                    };
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    dirs = new string[] {
                        AndroidCore.UserAndroidHomePath_WIN,
                        AndroidCore.SystemAndroidHomePath_WIN32,
                        AndroidCore.SystemAndroidHomePath_WIN64
                    };
                }
                else
                {
                    throw newUnsupported();
                }
                foreach (var dir in dirs)
                {
                    if (Directory.Exists(dir))
                    {
                        return new DirectoryInfo(dir);
                    }
                }
            }
            return null;
        }

        public static DirectoryInfo GetDefaultAndroidHome()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return new DirectoryInfo(AndroidCore.UserAndroidHomePath_OSX);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new DirectoryInfo(AndroidCore.UserAndroidHomePath_WIN);
            }
            else
            {
                throw newUnsupported();
            }
        }

        private static Exception newUnsupported()
        {
            return new NotImplementedException("Currently, AndroidResolver is only supported to macOS and Windows.");
        }

        public static FileInfo GetDebugKeyStore()
        {
            return new FileInfo(AndroidCore.UserDebugKeystorePath_ANY);
        }

        public static Uri GetSdkArchiveUri()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return AndroidCore.ArchiveUriOsX;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return AndroidCore.ArchiveUriWin;
            }
            else
            {
                throw newUnsupported();
            }
        }

    }
}
