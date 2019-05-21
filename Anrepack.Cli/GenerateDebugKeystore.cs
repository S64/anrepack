using System;
using System.IO;
using McMaster.Extensions.CommandLineUtils;

namespace Anrepack.Cli
{

    [Command(
        SUBCOMMAND_NAME,
        Description = "Generate \"debug.keystore\" to default location."
    )]
    public class GenerateDebugKeystore : IAnrepackCommand
    {
        public const string SUBCOMMAND_NAME = "generate-debug-keystore";

        public void OnExecute()
        {
            DirectoryInfo javaHome;
            if ((javaHome = PathResolver.GetJavaHome()) == null)
            {
                throw new AnrepackException("JDK not installed.");
            }

            var generator = new AndroidDebugKeystoreGenerator(javaHome);
            try
            {
                generator.Execute();
            }
            catch (KSGException e)
            {
                throw new AnrepackException(e.Message);
            }
        }

    }
}
