using System;
using McMaster.Extensions.CommandLineUtils;

namespace Anrepack.Cli
{

    [Command(
        "version",
        Description = "Show version."
    )]
    public class AnrepackVersion : IAnrepackCommand
    {

        [Option(
            "-q",
            CommandOptionType.NoValue
        )]
        bool Quiet { get; }

        public void OnExecute()
        {
            if (!Quiet)
            {
                Console.WriteLine($"{Program.AppName} {Program.AppVersion}");
            }
            else
            {
                Console.WriteLine($"{Program.AppVersion}");
            }
        }

    }

}
