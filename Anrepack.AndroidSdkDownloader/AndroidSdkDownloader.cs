using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Anrepack
{

    public class AndroidSdkDownloader
    {

        private const string ArchiveFilename = "sdk-tools.zip";
        private const string UnarchiveDestDirName = "sdk-tools-dest";

        private static readonly char DSC = Path.DirectorySeparatorChar;

        public readonly DirectoryInfo TempDir;
        private readonly WebClient web;

        public AndroidSdkDownloader(DirectoryInfo outputDir = null)
        {
            TempDir = new TempDirGenerator(outputDir).Generate();
            web = new WebClient();
        }

        public Task DownloadSdkToolsAsync()
        {
            return web.DownloadFileTaskAsync(
                AndroidResolver.GetSdkArchiveUri(),
                GetArchivePath()
            );
        }

        public void UnarchiveSdkTools()
        {
            using (var archive = ZipFile.Open(GetArchivePath(), ZipArchiveMode.Read))
            {
                archive.ExtractToDirectory(GetArchiveDestPath());
            }
        }

        public void MoveUnarchivedSdkToolsToDefaultDir()
        {
            var dest = GetDefaultDirectory();
            if (dest.Exists)
            {
                throw new InvalidOperationException("Android SDK is already installed.");
            }
            dest.Create();

            var src = new DirectoryInfo(GetArchiveDestPath());
            foreach (var dir in src.GetDirectories())
            {
                dir.MoveTo($"{dest.FullName}{DSC}{dir.Name}");
            }
            foreach (var file in src.GetFiles())
            {
                file.MoveTo($"{dest.FullName}{DSC}{file.Name}");
            }

            var sdkmgr = GetSdkManagerExecutable();
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                (new ShellConnector(new FileInfo("/bin/bash"), $"-c \"chmod +x '{sdkmgr}'\""))
                    .Execute();
            }
        }

        public DirectoryInfo GetDefaultDirectory()
        {
            return AndroidResolver.GetDefaultAndroidHome();
        }

        public FileInfo GetSdkManagerExecutable()
        {
            return new FileInfo($"{GetDefaultDirectory().FullName}{DSC}tools{DSC}bin{DSC}sdkmanager");
        }

        private string GetArchivePath()
        {
            return $"{TempDir.FullName}{DSC}{ArchiveFilename}";
        }

        private string GetArchiveDestPath()
        {
            return $"{TempDir.FullName}{DSC}{UnarchiveDestDirName}";
        }

    }

}
