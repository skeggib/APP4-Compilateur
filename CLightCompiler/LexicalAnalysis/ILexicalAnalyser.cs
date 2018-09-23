using System.Collections.Generic;

namespace LexicalAnalysis
{
    public interface ILexicalAnalyser
    {
        List<Token> Convert(string code);
    }
}
