using SyntaxAnalysis;

namespace SemanticAnalysis
{
    public interface ISemanticAnalyser
    {
        void Analyse(Node tree);
    }
}
