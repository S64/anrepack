using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Anrepack
{

    public class ApktoolDownloader
    {

        public static readonly FileInfo TempJarDest
            = new FileInfo($"{AndroidCore.HomeDir.FullName}{AndroidCore.DSC}.anrepack{AndroidCore.DSC}bin{AndroidCore.DSC}apktool.jar");

        private readonly WebClient web;

        public ApktoolDownloader()
        {
            web = new WebClient();
        }

        public Task Exec()
        {
            if (TempJarDest.Exists)
            {
                throw new ADException("Apktool is already installed.");
            }

            if (!TempJarDest.Directory.Exists)
            {
                TempJarDest.Directory.Create();
            }

            return web.DownloadFileTaskAsync(
                "https://bitbucket.org/iBotPeaches/apktool/downloads/apktool_2.4.0.jar",
                TempJarDest.FullName
            );
        }
    }

    public class ADException : Exception
    {

        public ADException(string msg) : base(msg) { }

    }

}
