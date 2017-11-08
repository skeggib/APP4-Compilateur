using SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneration
{
    public interface ICodeGenerator
    {
        string Generate(Node tree);
    }
}
