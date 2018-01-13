using System.Diagnostics;
using System.IO;
using System.Text;

namespace CLightCompiler.Tests
{
    public static class TestHelper
    {
        public static string Run(string asmCode)
        {
            using (FileStream stream = File.Create("asmCode"))
            {
                var bytes = Encoding.ASCII.GetBytes(asmCode);
                stream.Write(bytes, 0, bytes.Length);
            }
            Process process = new Process();
            process.StartInfo.FileName = "msm.exe";
            process.StartInfo.Arguments = "asmCode";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            //* Read the output (or the error)
            string output = process.StandardOutput.ReadToEnd();
            string err = process.StandardError.ReadToEnd();
            process.WaitForExit();
            return output + err;
        }

        public static string Compile(string code)
        {
            return CLightCompiler.Compile(code);
        }
    }
}
