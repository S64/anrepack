using System;
using System.IO;
using Microsoft.Scripting.Hosting;
using IronPython.Hosting;
using System.Linq;

namespace Anrepack.Cli
{
    public class PythonScriptRunner
    {

        private readonly ScriptEngine engine;

        private readonly FileInfo scriptFile;
        private readonly DirectoryInfo decodedApkDir;

        public PythonScriptRunner(FileInfo scriptFile, DirectoryInfo decodedApkDir, string[] argv)
        {
            this.scriptFile = scriptFile;
            this.decodedApkDir = decodedApkDir;
            {
                engine = Python.CreateEngine();
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    engine.Runtime.LoadAssembly(asm);
                }
                engine.Runtime.GetSysModule()
                    .SetVariable(
                    "argv",
                    new string[] { scriptFile.FullName }.Concat(argv).ToArray()
                );
            }
        }

        public void Execute()
        {
            var scope = engine.Runtime.CreateScope();

            var compiled = engine.CreateScriptSourceFromFile(scriptFile.FullName)
                .Compile();

            compiled.Execute(scope);

            scope.GetVariable("processDecodedApk")(
                Program.AppVersion,
                decodedApkDir.FullName
            );
        }

    }
}
