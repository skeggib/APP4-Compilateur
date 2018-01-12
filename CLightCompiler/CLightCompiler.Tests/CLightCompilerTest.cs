using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Text;
using LexicalAnalysis;
using SyntaxAnalysis;
using SemanticAnalysis;
using CodeGeneration;

namespace CLightCompiler.Tests
{
    [TestClass]
    public class CLightCompilerTest
    {
        [TestMethod]
        public void DisplayZero()
        {
            string code = "int main(){out 0;}";
            string result = run(compile(code));
            Assert.AreEqual("0", result);
        }


        private string run(string asmCode)
        {
            using(FileStream stream = File.Create("asmCode"))
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

        private string compile(string code)
        {
            var lexical = new LexicalAnalyser();
            var syntax = new SyntaxAnalyser();
            var semantic = new SemanticAnalyser();
            var generation = new MSMCodeGenerator();

            var tokens = lexical.Convert(code);
            var tree = syntax.Convert(tokens);
            semantic.Analyse(tree);
            return generation.Generate(tree);
        }

    }
}
