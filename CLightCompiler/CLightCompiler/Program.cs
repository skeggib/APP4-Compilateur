using CodeGeneration;
using LexicalAnalysis;
using SemanticAnalysis;
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
                Console.WriteLine("List of availables architecture:");
                Console.WriteLine("- msm (default)");
                Exit();
                return;
            }

            string inputPath = args[0];
            string outputPath = args[1];
            string architecture = args.Length > 2 ? args[2] : "msm";

            LexicalAnalyser lexical = new LexicalAnalyser();
            SyntaxAnalyser syntax = new SyntaxAnalyser();
            SemanticAnalyser semantics = new SemanticAnalyser();

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
                    SymbolsTable table = semantics.Analyse(tree);
                    //string asmCode = generator.Generate(tree);
                    string asmCode = ((MSMCodeGenerator)generator).Generate2(tree, semantics.Counter);

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
                catch (LexicalException e)
                {
                    DisplayError(cLightCode, e.Message, e.Offset);
                    Exit();
                }
                catch (SyntaxException e)
                {
                    DisplayError(cLightCode, e.Message, e.Offset);
                    Exit();
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

        private static void DisplayError(string code, string error, int offset)
        {
            int lineOffset = 0;
            int lineNumber = 0;
            for (int i = 0; i < code.Length && i < offset; i++)
            {
                if (code[i] != '\n')
                    lineOffset++;
                else
                {
                    lineNumber++;
                    lineOffset = 0;
                }
            }
            string line = code.Split('\n')[lineNumber];

            while (line[0] == ' ' || line[0] == '\t')
            {
                line = line.Substring(1);
                lineOffset--;
            }

            Console.WriteLine(error + ":\n");

            Console.WriteLine(line);

            for (int i = 0; i < lineOffset; i++)
                Console.Write("_");
            Console.Write("^");
            for (int i = 0; i < line.Length - lineOffset - 2; i++)
                Console.Write("_");
        }
    }
}
