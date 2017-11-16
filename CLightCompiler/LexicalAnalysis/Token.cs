using System;
using System.Collections.Generic;
using System.Text;

namespace LexicalAnalysis
{
    public class Token
    {
        /// <summary>
        /// Categorie du token
        /// </summary>
        public Tokens Category { get; set; }

        /// <summary>
        /// Identifiant du token, a une valeur uniquement si la categorie est TokIdent
        /// </summary>
        public string Ident { get; set; }

        /// <summary>
        /// Valeur du token, a une valeur uniquement si la categorie est TokValue
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Position du token dans le fichier source
        /// </summary>
        public int Offset { get; set; }

        private static Dictionary<string, Tokens> _keyWordsAssociations;
        /// <summary>
        /// Association des mot-cles aux categories de token
        /// </summary>
        public static Dictionary<string, Tokens> KeyWordsAssociations
        {
            get
            {
                if (_keyWordsAssociations == null)
                {
                    _keyWordsAssociations = new Dictionary<string, Tokens>();
                    _keyWordsAssociations.Add("if", Tokens.If);
                    _keyWordsAssociations.Add("else", Tokens.Else);
                    _keyWordsAssociations.Add("for", Tokens.For);
                    _keyWordsAssociations.Add("while", Tokens.While);
                    _keyWordsAssociations.Add("do", Tokens.Do);
                    _keyWordsAssociations.Add("break", Tokens.Break);
                    _keyWordsAssociations.Add("continue", Tokens.Continue);
                    _keyWordsAssociations.Add("return", Tokens.Return);
                    _keyWordsAssociations.Add("int", Tokens.Int);
                    _keyWordsAssociations.Add("void", Tokens.Void);
                    _keyWordsAssociations.Add("out", Tokens.Out);
                }

                return _keyWordsAssociations;
            }
        }

        private static Dictionary<string, Tokens> _specialCharactersAssociations;
        /// <summary>
        /// Association des caracteres speciaux aux categories de token
        /// </summary>
        public static Dictionary<string, Tokens> SpecialCharactersAssociations
        {
            get
            {
                if (_specialCharactersAssociations == null)
                {
                    _specialCharactersAssociations = new Dictionary<string, Tokens>();
                    _specialCharactersAssociations.Add("(", Tokens.OpeningParenthesis);
                    _specialCharactersAssociations.Add(")", Tokens.ClosingParenthesis);
                    _specialCharactersAssociations.Add("{", Tokens.OpeningBrace);
                    _specialCharactersAssociations.Add("}", Tokens.ClosingBrace);
                    _specialCharactersAssociations.Add("[", Tokens.OpeningBracket);
                    _specialCharactersAssociations.Add("]", Tokens.ClosingBracket);
                    _specialCharactersAssociations.Add(";", Tokens.Semicolon);
                    _specialCharactersAssociations.Add("=", Tokens.Assign);
                    _specialCharactersAssociations.Add("+", Tokens.Plus);
                    _specialCharactersAssociations.Add("-", Tokens.Minus);
                    _specialCharactersAssociations.Add("*", Tokens.Multiply);
                    _specialCharactersAssociations.Add("/", Tokens.Divide);
                    _specialCharactersAssociations.Add("%", Tokens.Modulo);
                    _specialCharactersAssociations.Add("==", Tokens.Equals);
                    _specialCharactersAssociations.Add("!=", Tokens.NotEquals);
                    _specialCharactersAssociations.Add("<", Tokens.LowerThan);
                    _specialCharactersAssociations.Add(">", Tokens.GreaterThan);
                    _specialCharactersAssociations.Add("<=", Tokens.LowerOrEqual);
                    _specialCharactersAssociations.Add(">=", Tokens.GreaterOrEqual);
                    _specialCharactersAssociations.Add("&&", Tokens.And);
                    _specialCharactersAssociations.Add("||", Tokens.Or);
                    _specialCharactersAssociations.Add("!", Tokens.Not);
                    _specialCharactersAssociations.Add("&", Tokens.Reference);
                    _specialCharactersAssociations.Add("@", Tokens.Pointer);
                    _specialCharactersAssociations.Add(",", Tokens.Comma);
                }

                return _specialCharactersAssociations;
            }
        }

        private static HashSet<char> _specialCharacters;
        /// <summary>
        /// Caracteres speciaux autorises
        /// </summary>
        public static HashSet<char> SpecialCharacters
        {
            get
            {
                if (_specialCharacters == null)
                {
                    _specialCharacters = new HashSet<char> { '(', ')', '{', '}', '[', ']', ';', '=', '+', '-', '*', '/', '%', '!', '<', '>', '&', '|', '@', ',' };
                }

                return _specialCharacters;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category">Categorie du token</param>
        /// <param name="offset">Position dans le fichier source</param>
        public Token(Tokens category, int offset)
        {
            Category = category;
            Ident = string.Empty;
            Value = string.Empty;
            Offset = offset;
        }

        public override string ToString()
        {
            return Category.ToString() + 
                (Category == Tokens.Ident || Category == Tokens.Value ? "(" : "") +
                (Category == Tokens.Ident ? Ident : "") +
                (Category == Tokens.Value ? Value : "") +
                (Category == Tokens.Ident || Category == Tokens.Value ? ")" : "") +
                "\t offset = " + Offset;
        }

        /// <summary>
        /// Transforme le token en code source
        /// </summary>
        /// <returns>Le code source correspondant au token</returns>
        public string ToCode()
        {
            if (Category == Tokens.Ident)
            {
                return Ident;
            }

            else if (Category == Tokens.Value)
            {
                return Value;
            }

            else if (Token.KeyWordsAssociations.ContainsValue(Category))
            {
                foreach (KeyValuePair<string, Tokens> item in Token.KeyWordsAssociations)
                {
                    if (item.Value == Category)
                        return item.Key;
                }
            }

            else if (Token.SpecialCharactersAssociations.ContainsValue(Category))
            {
                foreach (KeyValuePair<string, Tokens> item in Token.SpecialCharactersAssociations)
                {
                    if (item.Value == Category)
                        return item.Key;
                }
            }

            return "";
        }
    }
}
