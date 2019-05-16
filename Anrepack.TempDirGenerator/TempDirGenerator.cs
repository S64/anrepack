using System;
using System.IO;

namespace Anrepack
{

    public class TempDirGenerator
    {

        private static readonly char DSC = Path.DirectorySeparatorChar;

        private readonly DirectoryInfo OutputDir;

        public TempDirGenerator(DirectoryInfo outputDir = null)
        {
            this.OutputDir = outputDir;
        }

        public DirectoryInfo Generate()
        {
            DirectoryInfo tempDir;

            var id = Guid.NewGuid();
            if (OutputDir == null)
            {
                tempDir = new DirectoryInfo($"{Path.GetTempPath()}{DSC}{id}");
            }
            else
            {
                if (!OutputDir.Exists)
                {
                    throw new ArgumentException("Passed outputDir is invalid.");
                }
                tempDir = new DirectoryInfo($"{OutputDir.FullName}{DSC}{id}");
            }
            tempDir.Create();
            return tempDir;
        }

    }

}
