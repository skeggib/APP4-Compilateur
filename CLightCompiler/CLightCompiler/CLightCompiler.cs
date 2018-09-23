using CodeGeneration;
using LexicalAnalysis;
using SemanticAnalysis;
using SyntaxAnalysis;
using System.IO;
using System;

namespace CLightCompiler
{
    public class CLightCompiler
    {
        private readonly ILexicalAnalyser _lexicalAnalyser;
        private readonly ISyntaxAnalyser _syntaxAnalyser;
        private readonly ISemanticAnalyser _semanticAnalyser;
        private readonly ICodeGenerator _codeGenerator;

        public CLightCompiler(
            ILexicalAnalyser lexical, 
            ISyntaxAnalyser syntax, 
            ISemanticAnalyser semantic,
            ICodeGenerator generator)
        {
            _lexicalAnalyser = lexical ?? throw new ArgumentNullException(nameof(lexical));
            _syntaxAnalyser = syntax ?? throw new ArgumentNullException(nameof(syntax));
            _semanticAnalyser = semantic ?? throw new ArgumentNullException(nameof(semantic));
            _codeGenerator = generator ?? throw new ArgumentNullException(nameof(generator));
        }

        public string Compile(string code)
        {
            var allCode = GetStdCode() + code;

            var tokens = _lexicalAnalyser.Convert(allCode);
            var tree = _syntaxAnalyser.Convert(tokens);
            _semanticAnalyser.Analyse(tree);
            return _codeGenerator.Generate(tree);
        }

        public static string GetStdCode()
        {
            return File.ReadAllText("std.c") + "\n";
        }
    }
}
