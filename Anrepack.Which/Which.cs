using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Anrepack
{

    public class Which
    {

        private static char PathSeparator => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ';' : ':';

        public static string Path => Environment.GetEnvironmentVariable("PATH");

        public static List<DirectoryInfo> PrioritySortedPathes
        {
            get
            {
                return Path.Split(PathSeparator)
                    .Where(x => Directory.Exists(x))
                    .Select(x => new DirectoryInfo(x))
                    .ToList();
            }
        }

        public static FileInfo Find(string executable)
        {
            foreach (var path in PrioritySortedPathes)
            {
                foreach (var file in path.GetFiles())
                {
                    if (file.Name.Equals(executable))
                    {
                        return file;
                    }
                }
            }
            return null;
        }

    }

}
