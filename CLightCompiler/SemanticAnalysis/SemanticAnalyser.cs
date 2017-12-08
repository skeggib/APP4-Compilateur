using SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysis
{
    public class SemanticAnalyser
    {
        public int Counter { get; private set; }

        private SymbolsTable _table;

        public SemanticAnalyser()
        {
            _table = new SymbolsTable();
        }


        public SymbolsTable Analyse(Node tree)
        {
            AnalyseLoop(tree);
            return AnalyseSymbols(tree);
        }

        public SymbolsTable AnalyseSymbols(Node tree)
        {
            if (tree.Category == Nodes.Block)
            {
                _table.StartBlock();
                foreach (var child in tree.Childs)
                {
                    AnalyseSymbols(child);
                }
                _table.EndBlock();
            }

            else if (tree.Category == Nodes.DeclVar)
            {
                _table.AddSymbol(tree.Token).Slot = Counter++;
            }

            else if (tree.Category == Nodes.RefVar)
            {
                tree.Slot = _table.GetSymbol(tree.Token).Slot;
            }

            else if(tree.Category == Nodes.Assign)
            {
                tree.Slot = _table.GetSymbol(tree.Token).Slot;
                foreach (var child in tree.Childs)
                {
                    AnalyseSymbols(child);
                }
            }

            else if(tree.Category == Nodes.DeclFunc)
            {
                _table.AddSymbol(tree.Token);
                _table.StartBlock();
                foreach(var param in tree.Tokens)
                {
                    _table.AddSymbol(param).Slot = Counter++;
                }
                AnalyseSymbols(tree.Childs[0]);
                _table.EndBlock();
            }

            else if(tree.Category == Nodes.Call)
            {
                _table.GetSymbol(tree.Token);
            }

            else
            {
                foreach (var child in tree.Childs)
                {
                    AnalyseSymbols(child);
                }
            }

            return _table;
        }


        private void AnalyseLoop(Node tree)
        {
            if (tree.Category == Nodes.Loop)
                return;
            if (tree.Category == Nodes.Break || tree.Category == Nodes.Continue)
                throw new SemanticException(tree.Token.Offset, $"Break ou continue n'est pas dans une boucle");
            foreach (var child in tree.Childs)
                AnalyseLoop(child);
        }
    }
}
