using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntaxAnalysis;

namespace CodeGeneration
{
    public class MSMCodeGenerator : ICodeGenerator
    {
        public string Generate(Node tree)
        {
            string code = string.Empty;
            code += ".start\n";
            code += _generate(tree);
            code += "out.i\n";
            code += "halt\n";
            return code;
        }

        private  string _generate(Node tree)
        {
            string code = string.Empty;
            switch (tree.Category)
            {
                case NodeCategory.NodeConst:
                    code += $"push.i {tree.Value}\n";
                    break;

                case NodeCategory.NodeAddition:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"add.i\n";
                    break;

                case NodeCategory.NodeSubstraction:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"sub.i\n";
                    break;

                case NodeCategory.NodeMultiplication:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"mul.i\n";
                    break;

                case NodeCategory.NodeDivision:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"div.i\n";
                    break;

                case NodeCategory.NodeModulo:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"mod.i\n";
                    break;

                case NodeCategory.NodeNegative:
                    code += $"push.i 0\n";
                    code += _generate(tree.Childs[0]);
                    code += $"sub.i\n";
                    break;

                case NodeCategory.NodeAreEqual:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"cmpeq.i\n";
                    break;

                case NodeCategory.NodeAreNotEqual:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"cmpne.i\n";
                    break;

                case NodeCategory.NodeLowerThan:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"cmplt.i\n";
                    break;

                case NodeCategory.NodeLowerOrEqual:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"cmple.i\n";
                    break;

                case NodeCategory.NodeGreaterThan:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"cmpgt.i\n";
                    break;

                case NodeCategory.NodeGreaterOrEqual:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"cmpge.i\n";
                    break;

                case NodeCategory.NodeLogicOr:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"or.i\n";
                    break;

                case NodeCategory.NodeLogicAnd:
                    code += _generate(tree.Childs[0]);
                    code += _generate(tree.Childs[1]);
                    code += $"and.i\n";
                    break;

                default:
                    throw new NotImplementedException($"Not implemented node ({tree.Category})");
            }
            return code;
        }
    }
}
