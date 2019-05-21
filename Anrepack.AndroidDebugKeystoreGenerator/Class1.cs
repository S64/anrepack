using System;
using System.IO;

namespace Anrepack
{

    public class AndroidDebugKeystoreGenerator
    {

        private static readonly char DSC = Path.DirectorySeparatorChar;

        private readonly DirectoryInfo JavaHome;

        public AndroidDebugKeystoreGenerator(DirectoryInfo javaHome)
        {
            this.JavaHome = javaHome;
        }

        public void Execute()
        {
            var keystore = AndroidResolver.GetDebugKeyStore();
            if (keystore.Exists)
            {
                throw new KSGException("debug.keystore already exists.");
            }

            (new ShellConnector(
               new FileInfo($"{JavaHome.FullName}{DSC}bin{DSC}keytool"),
               $"-genkey -v -keystore {keystore.FullName} -storepass android -alias androiddebugkey -keypass android -keyalg RSA -keysize 2048 -validity 10000 -dname \"C=US, O=Android, CN=Android Debug\""
            )).Execute();
        }

    }

    public class KSGException : Exception
    {

        public KSGException(string msg) : base(msg) { }

    }

}
