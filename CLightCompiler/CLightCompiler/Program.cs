using LexicalAnalysis;
using SemanticAnalysis;
using SyntaxAnalysis;
using System;
using System.IO;
using CodeGeneration;

namespace CLightCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: CLightCompiler.exe <input_file> <output_file>");
                return;
            }
            
            var inputPath = args[0];
            var outputPath = args[1];

            try
            {
                string cLightCode = null;

                using (var reader = new StreamReader(inputPath))
                {
                    cLightCode += reader.ReadToEnd();
                }

                try
                {
                    var compiler = new CLightCompiler(
                        new LexicalAnalyser(), 
                        new SyntaxAnalyser(), 
                        new SemanticAnalyser(), 
                        new MSMCodeGenerator());

                    var asmCode = compiler.Compile(cLightCode);

                    try
                    {
                        using (var writer = new StreamWriter(outputPath, false))
                        {
                            writer.Write(asmCode);
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Cannot write file " + outputPath);
                    }
                }
                catch (LexicalException e)
                {
                    DisplayError(CLightCompiler.GetStdCode() + cLightCode, e.Message, e.Offset);
                }
                catch (SyntaxException e)
                {
                    DisplayError(CLightCompiler.GetStdCode() + cLightCode, e.Message, e.Offset);
                }
                catch (SemanticException e)
                {
                    DisplayError(CLightCompiler.GetStdCode() + cLightCode, e.Message, e.Offset);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Cannot compile: "+ e.Message + "\n" + e.StackTrace);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Cannot read file " + inputPath);
            }
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

            Console.WriteLine("");
        }
    }
}
