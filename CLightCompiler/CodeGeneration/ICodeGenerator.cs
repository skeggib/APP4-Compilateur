using SyntaxAnalysis;

namespace CodeGeneration
{
    public interface ICodeGenerator
    {
        string Generate(Node tree);
    }
}
