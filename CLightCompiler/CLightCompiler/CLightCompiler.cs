using CodeGeneration;
using LexicalAnalysis;
using SemanticAnalysis;
using SyntaxAnalysis;
using System.IO;
using System;

namespace CLightCompiler
{
    public static class CLightCompiler
    {
        public static string Compile(string code)
        {
            var lexical = new LexicalAnalyser();
            var syntax = new SyntaxAnalyser();
            var semantics = new SemanticAnalyser();
            var generator = new MSMCodeGenerator();

            var allCode = GetStdCode() + code;

            var tokens = lexical.Convert(allCode);
            var tree = syntax.Convert(tokens);
            semantics.Analyse(tree);
            return generator.Generate(tree);
        }

        public static string GetStdCode()
        {
            return File.ReadAllText("std.c") + "\n";
        }
    }
}
