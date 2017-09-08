using LexicalAnalysis;
using Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            SymbolsTable table = new SymbolsTable();

            table.AddSymbol(new Token(TokenCategory.TokIdent, 0) { Ident = "var1" });
            table.StartBlock();
            table.AddSymbol(new Token(TokenCategory.TokIdent, 0) { Ident = "var2" });
            table.GetSymbol(new Token(TokenCategory.TokIdent, 0) { Ident = "var1" });
            table.EndBlock();
        }
    }
}
