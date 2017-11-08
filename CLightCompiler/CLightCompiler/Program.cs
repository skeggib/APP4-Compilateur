using CodeGeneration;
using LexicalAnalysis;
using SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLightCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine($"Usage: {args[0]} <input_file> <output_file> [asm]");
                Console.WriteLine("List of availables asm:");
                Console.WriteLine("- msm (default)");
                return;
            }

            string inputPath = args[1];
            string outputPath = args[2];
            string asmName = args.Length > 3 ? args[3] : "msm";

            LexicalAnalyser lexical = new LexicalAnalyser();
            SyntaxAnalyser syntax = new SyntaxAnalyser();

            ICodeGenerator generator = null;
            switch (asmName)
            {
                case "msm":
                    generator = new MSMCodeGenerator();
                    break;
                default:
                    Console.WriteLine("Unknown asm");
                    return;
            }

            try
            {
                using (StreamReader reader = new StreamReader(inputPath))
                {
                    string cLightCode = reader.ReadToEnd();
                }

                try
                {

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Cannot compile: {e.Message}");
                    return;
                }

                try
                {

                }
                catch (Exception)
                {
                    Console.WriteLine($"Cannot write file {outputPath}");
                    return;
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"Cannot read file {inputPath}");
                return;
            }
        }
    }
}
