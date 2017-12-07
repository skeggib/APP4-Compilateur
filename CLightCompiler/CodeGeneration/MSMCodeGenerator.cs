using System;
using SyntaxAnalysis;
using System.Collections;
using System.Collections.Generic;

namespace CodeGeneration
{
    public class MSMCodeGenerator : ICodeGenerator
    {
        private int _indent = 0;
        private string GetIndentString(int offset = 0)
        {
            string str = String.Empty;
            for (int i = 0; i < _indent + offset; i++)
                str += "\t";
            return str;
        }

        public string Generate(Node tree, int nbVars)
        {
            string code = string.Empty;
            code += ".start\n";
            for(int i = 0; i < nbVars; i++)
                code += "push.i 0\n";
            code += _generate(tree);
            code += "halt\n";
            return code;
        }

        private  string _generate(Node tree)
        {
            string code = string.Empty;
            switch (tree.Category)
            {
                case Nodes.Const:
                    code += $"push.i {tree.Token.Value}\t\t\t;{GetIndentString()}const {tree.Token.Value} ->\n";
                    break;

                case Nodes.RefVar:
                    code += $"get {tree.Slot}\t\t\t\t;{GetIndentString()}var {tree.Token.Ident} ->\n";
                    break;

                case Nodes.Addition:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"add.i\t\t\t\t;{GetIndentString()}+\n";
                    break;

                case Nodes.Substraction:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"sub.i\t\t\t\t;{GetIndentString()}-\n";
                    break;

                case Nodes.Multiplication:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"mul.i\t\t\t\t;{GetIndentString()}*\n";
                    break;

                case Nodes.Division:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"div.i\t\t\t\t;{GetIndentString()}/\n";
                    break;

                case Nodes.Modulo:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"mod.i\t\t\t\t;{GetIndentString()}%\n";
                    break;

                case Nodes.Negative:
                    code += $"push.i 0\t\t\t;{GetIndentString()}const 0 ->\n";
                    code += _generate(tree.Childs[0]);
                    code += $"sub.i\t\t\t\t;{GetIndentString()}-\n";
                    break;

                case Nodes.AreEqual:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"cmpeq.i\t\t\t\t;{GetIndentString()}==\n";
                    break;

                case Nodes.AreNotEqual:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"cmpne.i\t\t\t\t;{GetIndentString()}!=\n";
                    break;

                case Nodes.LowerThan:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"cmplt.i\t\t\t\t;{GetIndentString()}<\n";
                    break;

                case Nodes.LowerOrEqual:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"cmple.i\t\t\t\t;{GetIndentString()}<=\n";
                    break;

                case Nodes.GreaterThan:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"cmpgt.i\t\t\t\t;{GetIndentString()}>\n";
                    break;

                case Nodes.GreaterOrEqual:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"cmpge.i\t\t\t\t;{GetIndentString()}>=\n";
                    break;

                case Nodes.And:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"and.i\t\t\t\t;{GetIndentString()}&&\n";
                    break;

                case Nodes.Or:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"or.i\t\t\t\t;{GetIndentString()}||\n";
                    break;

                case Nodes.Not: // TODO
                    throw new NotImplementedException();
                    break;

                case Nodes.Assign:
                    code += _generate(tree.Childs[0]);
                    code += $"set {tree.Slot} \t\t\t\t;{GetIndentString()}var {tree.Token.Ident} <-\n";
                    break;

                case Nodes.Block:
                    for(int i = 0; i < tree.Childs.Count; i++)
                    {
                        code += _generate(tree.Childs[i]);
                    }
                    break;

                case Nodes.Condition:
                    string l1 = GetConditionElseLabel();
                    string l2 = GetConditionEndLabel();

                    _indent++;

                    code += $"\t\t\t\t\t;{GetIndentString(-1)}if\n";
                    code += _generate(tree.Childs[0]);
                    code += $"jumpf {l1}\t;\n";
                    code += $"\t\t\t\t\t;{GetIndentString(-1)}then\n";
                    code += _generate(tree.Childs[1]);
                    if (tree.Childs.Count > 2)
                    {
                        code += $"jump " + l2 + "\t;\n";
                        code += $"\t\t\t\t\t;{GetIndentString(-1)}else\n";
                    }
                    code += $"." + l1 + "\t\t;\n";
                    if (tree.Childs.Count > 2)
                    {
                        code += _generate(tree.Childs[2]);
                        code += "." + l2 + "\t\t;\n";
                    }
                    code += $"\t\t\t\t\t;{GetIndentString(-1)}endif\n";

                    _indent--;
                    break;

                case Nodes.DeclVar:
                    break;

                case Nodes.Drop:
                    code += _generate(tree.Childs[0]);
                    code += "drop\n";
                    break;

                case Nodes.Out:
                    code += _generate(tree.Childs[0]);
                    code += $"out.i\t\t\t\t;{GetIndentString()}disp\n"; // TODO out.c
                    break;

                case Nodes.Loop:
                    code += $".{GetLoopBeginningLabel()}\t\t;{GetIndentString()}loop\n";
                    _indent++;
                    code += _generate(tree.Childs[0]);
                    _indent--;
                    code += $".{GetLoopEndLabel()}\t\t;{GetIndentString()}endloop\n";
                    break;

                case Nodes.Continue:
                    code += $"jump {GetCurrentLoopBeginningLabel()}\t;{GetIndentString()}continue\n";
                    break;

                case Nodes.Break:
                    code += $"jump {GetCurrentLoopEndLabel()}\t;{GetIndentString()}break\n";
                    break;

                case Nodes.DeclFunc:
                    code += $".{tree.Token.Ident}\n";
                    for(int i=0; i< tree.Childs.Count; ++i)
                        code += $"push.i 999\n"; // eviter de mettre 0 pour l'initialisation
                    code+=_generate(tree.Childs[0]);
                    code += "push.i 0\n";
                    code += "ret\n";
                    break;

                case Nodes.Call:
                    code += $"prep {tree.Token.Ident}\n";
                    foreach(var child in tree.Childs)
                        code += _generate(child);
                    code += $"call {tree.Childs.Count}\n"; 
                    break;

                case Nodes.Return:
                    code += _generate(tree.Childs[0]);
                    code += "ret\n";
                    break;

                case Nodes.Program:
                    code += ".start\n";
                    code += "prep main\n";
                    code += "call 0\n";
                    code += "halt\n";
                    foreach (var child in tree.Childs)
                        code += _generate(child);
                    break;
                default:
                    throw new NotImplementedException($"Not implemented node ({tree.Category})");
            }
            return code;
        }

        private int _conditionElseLabelCounter = 0;
        private string GetConditionElseLabel()
        {
            return "else___" + String.Format("{0:0000}", _conditionElseLabelCounter++);
        }

        private int _conditionEndLabelCounter = 0;
        private string GetConditionEndLabel()
        {
            return "endif__" + String.Format("{0:0000}", _conditionEndLabelCounter++);
        }

        private int _loopNumber;
        private Stack<int> _loopNumberStack = new Stack<int>();

        private string GetLoopBeginningLabel()
        {
            _loopNumberStack.Push(_loopNumber++);
            return GetCurrentLoopBeginningLabel();
        }

        private string GetCurrentLoopBeginningLabel()
        {
            return $"loop___{String.Format("{0:0000}", _loopNumberStack.Peek())}";
        }

        private string GetLoopEndLabel()
        {
            string label = GetCurrentLoopEndLabel();
            _loopNumberStack.Pop();
            return label;
        }

        private string GetCurrentLoopEndLabel()
        {
            return $"endloop{String.Format("{0:0000}", _loopNumberStack.Peek())}";
        }
    }
}
