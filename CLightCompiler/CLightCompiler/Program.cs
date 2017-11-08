using CodeGeneration;
using LexicalAnalysis;
using SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.IO;

namespace CLightCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine($"Usage: CLightCompiler.exe <input_file> <output_file> [architecture]");
                Console.WriteLine("List of availables asm:");
                Console.WriteLine("- msm (default)");
                Exit();
                return;
            }

            string inputPath = args[0];
            string outputPath = args[1];
            string architecture = args.Length > 2 ? args[2] : "msm";

            LexicalAnalyser lexical = new LexicalAnalyser();
            SyntaxAnalyser syntax = new SyntaxAnalyser();

            ICodeGenerator generator = null;
            switch (architecture)
            {
                case "msm":
                    generator = new MSMCodeGenerator();
                    break;
                default:
                    Console.WriteLine($"Unknown architecture: {architecture}");
                    Exit();
                    return;
            }

            try
            {
                string cLightCode = null;
                using (StreamReader reader = new StreamReader(inputPath))
                {
                    cLightCode = reader.ReadToEnd();
                }

                try
                {
                    List<Token> tokens = lexical.Convert(cLightCode);
                    Node tree = syntax.Convert(tokens);
                    string asmCode = generator.Generate(tree);

                    try
                    {
                        using (StreamWriter writer = new StreamWriter(outputPath, false))
                        {
                            writer.Write(asmCode);
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Cannot write file {outputPath}");
                        Exit();
                        return;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Cannot compile: {e.Message}");
                    Exit();
                    return;
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"Cannot read file {inputPath}");
                Exit();
                return;
            }
        }

        private static void Exit()
        {
            Console.ReadKey();
        }
    }
}
