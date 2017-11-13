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
            if (tree.Category == Nodes.Block)
            {
                _table.StartBlock();
                foreach (var child in tree.Childs)
                {
                    Analyse(child);
                }
               // _table.EndBlock();
            }

            else if (tree.Category == Nodes.Declaration)
            {
                _table.AddSymbol(tree.Token).Slot = Counter++;
            }

            else if (tree.Category == Nodes.RefVar || tree.Category == Nodes.Assign)
            {
                tree.Slot = _table.GetSymbol(tree.Token).Slot;
            }
            else
            {
                foreach (var child in tree.Childs)
                {
                    Analyse(child);
                }
            }

            return _table;
        }
    }
}
