using System;
using System.IO;

namespace Anrepack
{

    public class AndroidCore
    {

        public static readonly DirectoryInfo HomeDir
            = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

        private static DirectoryInfo Win32ProgramFiles()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            return Directory.Exists(path) ? new DirectoryInfo(path) : null;
        }

        private static DirectoryInfo Win64ProgramFiles()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            return Directory.Exists(path) ? new DirectoryInfo(path) : null;
        }

        public static readonly char DSC = Path.DirectorySeparatorChar;

        public readonly static string UserAndroidHomePath_OSX = $"{HomeDir.FullName}{DSC}Library{DSC}Android{DSC}sdk";

        public readonly static string SystemAndroidHomePath_OSX = $"{DSC}Library{DSC}Android{DSC}sdk";

        public readonly static string UserDebugKeystorePath_ANY = $"{HomeDir.FullName}{DSC}.android{DSC}debug.keystore";

        public readonly static string UserAndroidHomePath_WIN = $"{HomeDir.FullName}{DSC}AppData{DSC}Local{DSC}Android{DSC}sdk";

        public static string SystemAndroidHomePath_WIN32()
        {
            if (Win32ProgramFiles() != null)
            {
                return $"{Win32ProgramFiles().FullName}{DSC}Android{DSC}android-sdk";
            }
            return null;
        }

        public static string SystemAndroidHomePath_WIN64()
        {
            if (Win64ProgramFiles() != null)
            {
                return $"{Win64ProgramFiles().FullName}{DSC}Android{DSC}android-sdk";
            }
            return null;
        }

        public static readonly Uri ArchiveUriOsX
            = new Uri("https://dl.google.com/android/repository/sdk-tools-darwin-4333796.zip");

        public static readonly Uri ArchiveUriWin
            = new Uri("https://dl.google.com/android/repository/sdk-tools-windows-4333796.zip");

    }

}
