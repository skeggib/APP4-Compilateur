using System;
using System.Collections.Generic;
using System.Text;

namespace LexicalAnalysis
{
    public enum TokenCategory
    {
        TokIdent,
        TokValue,
        TokIf,
        TokElse,
        TokFor,
        TokWhile,
        TokDo,
        TokBreak,
        TokContinue,
        TokReturn,
        TokInt,
        TokVoid,
        TokOpeningParenthesis,
        TokClosingParenthesis,
        TokOpeningBrace,
        TokClosingBrace,
        TokOpeningBracket,
        TokClosingBracket,
        TokSemicolon,
        TokPlus,
        TokMinus,
        TokMultiply,
        TokDivide,
        TokModulo,
        TokEquals,
        TokNotEquals,
        TokLowerThan,
        TokGreaterThan,
        TokLowerOrEquals,
        TokGreaterOrEquals,
        TokAnd,
        TokOr,
        TokNot,
        TokReference,
        TokPointer,
        TokAssign,
        TokComma
    }
}
