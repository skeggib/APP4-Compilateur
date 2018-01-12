using CodeGeneration;
using LexicalAnalysis;
using SemanticAnalysis;
using SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            var allCode = File.ReadAllText("std.c") + code;

            var tree = syntax.Convert(lexical.Convert(allCode));
            semantics.Analyse(tree);
            return generator.Generate(tree);
        }
    }
}
