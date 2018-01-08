using System.Collections.Generic;

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
        
        /// <summary>
        /// Association des mot-cles aux categories de token
        /// </summary>
        public static readonly Dictionary<string, Tokens> KeyWordsAssociations = new Dictionary<string, Tokens>
        {
            {"if", Tokens.If},
            {"else", Tokens.Else},
            {"for", Tokens.For},
            {"while", Tokens.While},
            {"do", Tokens.Do},
            {"break", Tokens.Break},
            {"continue", Tokens.Continue},
            {"return", Tokens.Return},
            {"int", Tokens.Int},
            {"void", Tokens.Void},
            {"out", Tokens.Out}
        };
        
        /// <summary>
        /// Association des caracteres speciaux aux categories de token
        /// </summary>
        public static readonly Dictionary<string, Tokens> SpecialCharactersAssociations = new Dictionary<string, Tokens>
        {
            {"(", Tokens.OpeningParenthesis},
            {")", Tokens.ClosingParenthesis},
            {"{", Tokens.OpeningBrace},
            {"}", Tokens.ClosingBrace},
            {"[", Tokens.OpeningBracket},
            {"]", Tokens.ClosingBracket},
            {";", Tokens.Semicolon},
            {"=", Tokens.Assign},
            {"+", Tokens.Plus},
            {"-", Tokens.Minus},
            {"*", Tokens.Asterisk},
            {"/", Tokens.Divide},
            {"%", Tokens.Modulo},
            {"==", Tokens.Equals},
            {"!=", Tokens.NotEquals},
            {"<", Tokens.LowerThan},
            {">", Tokens.GreaterThan},
            {"<=", Tokens.LowerOrEqual},
            {">=", Tokens.GreaterOrEqual},
            {"&&", Tokens.And},
            {"||", Tokens.Or},
            {"!", Tokens.Not},
            {"&", Tokens.Reference},
            {",", Tokens.Comma}
        };
        

        /// <summary>
        /// Caracteres speciaux autorises
        /// </summary>
        public static readonly HashSet<char> SpecialCharacters = new HashSet<char>
        {
            '(',
            ')',
            '{',
            '}',
            '[',
            ']',
            ';',
            '=',
            '+',
            '-',
            '*',
            '/',
            '%',
            '!',
            '<',
            '>',
            '&',
            '|',
            '@',
            ','
        };
        
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

            else if (KeyWordsAssociations.ContainsValue(Category))
            {
                foreach (KeyValuePair<string, Tokens> item in KeyWordsAssociations)
                {
                    if (item.Value == Category)
                        return item.Key;
                }
            }

            else if (SpecialCharactersAssociations.ContainsValue(Category))
            {
                foreach (KeyValuePair<string, Tokens> item in SpecialCharactersAssociations)
                {
                    if (item.Value == Category)
                        return item.Key;
                }
            }

            return "";
        }
    }
}
