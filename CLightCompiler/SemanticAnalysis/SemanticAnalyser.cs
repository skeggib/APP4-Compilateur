using SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysis
{
    public class SemanticAnalyser : ISemanticAnalyser
    {
        public int Counter { get; private set; }

        private SymbolsTable _table;

        public SemanticAnalyser()
        {
            _table = new SymbolsTable();
        }


        public void Analyse(Node tree)
        {
            AnalyseLoop(tree);
            AnalyseSymbols(tree);
        }

        public void AnalyseSymbols(Node tree)
        {
            if (tree == null)
                return;

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
                tree.VarCount = Counter - tree.Tokens.Count;
                Counter = 0;
            }

            else if(tree.Category == Nodes.Call)
            {
                _table.GetSymbol(tree.Token);
                foreach (var param in tree.Childs)
                {
                    if (param.Token != null && param.Token.Category == LexicalAnalysis.Tokens.Ident)
                        param.Slot = _table.GetSymbol(param.Token).Slot;
                }
            }

            else if (tree.Category == Nodes.Indir)
            {
                tree.Slot = _table.GetSymbol(tree.Token).Slot;
            }

            else if (tree.Category == Nodes.IndirSet)
            {
                tree.Slot = _table.GetSymbol(tree.Token).Slot;
                AnalyseSymbols(tree.Childs[0]);
            }

            else if (tree.Category == Nodes.Index)
            {
                tree.Slot = _table.GetSymbol(tree.Token).Slot;
                AnalyseSymbols(tree.Childs[0]);
            }

            else if (tree.Category == Nodes.IndexSet)
            {
                tree.Slot = _table.GetSymbol(tree.Token).Slot;
                AnalyseSymbols(tree.Childs[0]);
                AnalyseSymbols(tree.Childs[1]);
            }

            else
            {
                foreach (var child in tree.Childs)
                {
                    AnalyseSymbols(child);
                }
            }
        }


        private void AnalyseLoop(Node tree)
        {
            if (tree.Category == Nodes.Loop)
                return;
            if (tree.Category == Nodes.Break || tree.Category == Nodes.Continue)
                throw new SemanticException(tree.Token.Offset, "Break ou continue n'est pas dans une boucle");
            foreach (var child in tree.Childs)
                AnalyseLoop(child);
        }
    }
}
