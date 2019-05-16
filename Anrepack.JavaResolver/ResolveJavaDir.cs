using System;
using System.IO;

namespace Anrepack
{

    public class JavaResolver
    {

        public static DirectoryInfo ResolveJavaDir()
        {
            {
                var env = Environment.GetEnvironmentVariable("JAVA_HOME");
                if (env != null)
                {
                    return new DirectoryInfo(env);
                }
            }
            {
                var java = Which.Find("java");
                if (java == null)
                {
                    return null;
                }
                return java.Directory.Parent; // ./../
            }
        }

    }

}
