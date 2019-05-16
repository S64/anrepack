using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Anrepack
{

    public class ShellConnector
    {

        private readonly Process proc;

        public ShellConnector(
            FileInfo executable,
            string args
        )
        {
            proc = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = executable.FullName,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardError = false,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = false,
                }
            };
        }

        public void Execute()
        {
            proc.Start();
            proc.WaitForExit();
            int exitCode;
            if ((exitCode = proc.ExitCode) != 0)
            {
                throw new InvalidOperationException($"Process thrown exit code {exitCode}.");
            }
        }

    }

}
