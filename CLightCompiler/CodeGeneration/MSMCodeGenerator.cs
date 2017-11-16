using System;
using SyntaxAnalysis;

namespace CodeGeneration
{
    public class MSMCodeGenerator : ICodeGenerator
    {
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
                    code += $"push.i {tree.Token.Value}\n";
                    break;

                case Nodes.RefVar:
                    code += $"get {tree.Slot}\n";
                    break;

                case Nodes.Addition:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"add.i\n";
                    break;

                case Nodes.Substraction:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"sub.i\n";
                    break;

                case Nodes.Multiplication:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"mul.i\n";
                    break;

                case Nodes.Division:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"div.i\n";
                    break;

                case Nodes.Modulo:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"mod.i\n";
                    break;

                case Nodes.Negative:
                    code += $"push.i 0\n";
                    code += _generate(tree.Childs[0]);
                    code += $"sub.i\n";
                    break;

                case Nodes.AreEqual:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"cmpeq.i\n";
                    break;

                case Nodes.AreNotEqual:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"cmpne.i\n";
                    break;

                case Nodes.LowerThan:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"cmplt.i\n";
                    break;

                case Nodes.LowerOrEqual:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"cmple.i\n";
                    break;

                case Nodes.GreaterThan:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"cmpgt.i\n";
                    break;

                case Nodes.GreaterOrEqual:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"cmpge.i\n";
                    break;

                case Nodes.And:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"and.i\n";
                    break;

                case Nodes.Or:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"or.i\n";
                    break;

                case Nodes.Not: // TODO
                    throw new NotImplementedException();
                    break;

                case Nodes.Assign:
                    code += _generate(tree.Childs[0]);
                    code += $"set {tree.Slot}\n";
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

                    code += _generate(tree.Childs[0]);
                    code += "jumpf " + l1 + "\n";
                    code += _generate(tree.Childs[1]);
                    if (tree.Childs.Count > 2)
                        code += "jump " + l2 + "\n";
                    code += l1 + "\n";
                    if (tree.Childs.Count > 2)
                    {
                        code += _generate(tree.Childs[2]);
                        code += l2 + "\n";
                    }
                    break;

                case Nodes.Declaration:
                    break;

                default:
                    throw new NotImplementedException($"Not implemented node ({tree.Category})");
            }
            return code;
        }

        private int _conditionElseLabelCounter = 0;
        private string GetConditionElseLabel()
        {
            return ".condElse" + _conditionElseLabelCounter++;
        }

        private int _conditionEndLabelCounter = 0;
        private string GetConditionEndLabel()
        {
            return ".condEnd" + _conditionEndLabelCounter++;
        }
    }
}
