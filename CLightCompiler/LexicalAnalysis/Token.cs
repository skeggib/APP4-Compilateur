using System;
using System.Collections.Generic;
using System.Text;

namespace LexicalAnalysis
{
    public class Token
    {
        public TokenCategory Category { get; set; }
        public string Ident { get; set; }
        public string Value { get; set; }
        public int Offset { get; set; }

        private static Dictionary<string, TokenCategory> _keyWordsAssociations;
        public static Dictionary<string, TokenCategory> KeyWordsAssociations
        {
            get
            {
                if (_keyWordsAssociations == null)
                {
                    _keyWordsAssociations = new Dictionary<string, TokenCategory>();
                    _keyWordsAssociations.Add("if", TokenCategory.TokIf);
                    _keyWordsAssociations.Add("else", TokenCategory.TokElse);
                    _keyWordsAssociations.Add("for", TokenCategory.TokFor);
                    _keyWordsAssociations.Add("while", TokenCategory.TokWhile);
                    _keyWordsAssociations.Add("do", TokenCategory.TokDo);
                    _keyWordsAssociations.Add("break", TokenCategory.TokBreak);
                    _keyWordsAssociations.Add("continue", TokenCategory.TokContinue);
                    _keyWordsAssociations.Add("return", TokenCategory.TokReturn);
                    _keyWordsAssociations.Add("int", TokenCategory.TokInt);
                    _keyWordsAssociations.Add("void", TokenCategory.TokVoid);
                }

                return _keyWordsAssociations;
            }
        }

        private static Dictionary<string, TokenCategory> _specialCharactersAssociations;
        public static Dictionary<string, TokenCategory> SpecialCharactersAssociations
        {
            get
            {
                if (_specialCharactersAssociations == null)
                {
                    _specialCharactersAssociations = new Dictionary<string, TokenCategory>();
                    _specialCharactersAssociations.Add("(", TokenCategory.TokOpeningParenthesis);
                    _specialCharactersAssociations.Add(")", TokenCategory.TokClosingParenthesis);
                    _specialCharactersAssociations.Add("{", TokenCategory.TokOpeningBrace);
                    _specialCharactersAssociations.Add("}", TokenCategory.TokClosingBrace);
                    _specialCharactersAssociations.Add("[", TokenCategory.TokOpeningBracket);
                    _specialCharactersAssociations.Add("]", TokenCategory.TokClosingBracket);
                    _specialCharactersAssociations.Add(";", TokenCategory.TokSemicolon);
                    _specialCharactersAssociations.Add("=", TokenCategory.TokAssign);
                    _specialCharactersAssociations.Add("+", TokenCategory.TokPlus);
                    _specialCharactersAssociations.Add("-", TokenCategory.TokMinus);
                    _specialCharactersAssociations.Add("*", TokenCategory.TokMultiply);
                    _specialCharactersAssociations.Add("/", TokenCategory.TokDivide);
                    _specialCharactersAssociations.Add("%", TokenCategory.TokModulo);
                    _specialCharactersAssociations.Add("==", TokenCategory.TokEquals);
                    _specialCharactersAssociations.Add("!=", TokenCategory.TokNotEquals);
                    _specialCharactersAssociations.Add("<", TokenCategory.TokLowerThan);
                    _specialCharactersAssociations.Add(">", TokenCategory.TokGreaterThan);
                    _specialCharactersAssociations.Add("<=", TokenCategory.TokLowerOrEquals);
                    _specialCharactersAssociations.Add(">=", TokenCategory.TokGreaterOrEquals);
                    _specialCharactersAssociations.Add("&&", TokenCategory.TokAnd);
                    _specialCharactersAssociations.Add("||", TokenCategory.TokOr);
                    _specialCharactersAssociations.Add("!", TokenCategory.TokNot);
                    _specialCharactersAssociations.Add("&", TokenCategory.TokReference);
                    _specialCharactersAssociations.Add("@", TokenCategory.TokPointer);
                }

                return _specialCharactersAssociations;
            }
        }

        public Token(TokenCategory category, int offset)
        {
            Category = category;
            Ident = string.Empty;
            Value = string.Empty;
            Offset = offset;
        }

        public override string ToString()
        {
            return Category.ToString() + 
                (Category == TokenCategory.TokIdent || Category == TokenCategory.TokValue ? "(" : "") +
                (Category == TokenCategory.TokIdent ? Ident : "") +
                (Category == TokenCategory.TokValue ? Value : "") +
                (Category == TokenCategory.TokIdent || Category == TokenCategory.TokValue ? ")" : "") +
                "\t offset = " + Offset;
        }

        public string ToCode()
        {
            if (Category == TokenCategory.TokIdent)
            {
                return Ident;
            }

            else if (Category == TokenCategory.TokValue)
            {
                return Value;
            }

            else if (Token.KeyWordsAssociations.ContainsValue(Category))
            {
                foreach (KeyValuePair<string, TokenCategory> item in Token.KeyWordsAssociations)
                {
                    if (item.Value == Category)
                        return item.Key;
                }
            }

            else if (Token.SpecialCharactersAssociations.ContainsValue(Category))
            {
                foreach (KeyValuePair<string, TokenCategory> item in Token.SpecialCharactersAssociations)
                {
                    if (item.Value == Category)
                        return item.Key;
                }
            }

            return "";
        }
    }
}
