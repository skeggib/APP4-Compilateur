using System.Collections.Generic;
using System.Linq;

namespace LexicalAnalysis
{
    /// <summary>
    /// Lexical analyser
    /// </summary>
    public class LexicalAnalyser : ILexicalAnalyser
    {
        /// <summary>
        /// Converts CLight code to a tokens list
        /// </summary>
        /// <param name="code">CLight code</param>
        /// <returns>The tokens list</returns>
        /// <exception cref="LexicalException"></exception>
        public List<Token> Convert(string code)
        {
            var tokens = new List<Token>();

            for (var i = 0; i < code.Length; i++)
            {
                // Si c'est un caractere alphabetique ou underscore, alors c'est un mot-cle, une fonction ou une variable
                if (IsAlphabeticalCharacter(code[i]) || code[i] == '_')
                {
                    var word = string.Empty;
                    var offset = i;

                    // On lit le mot en entier
                    word += code[i];
                    while (i < code.Length - 1 && (IsAlphabeticalCharacter(code[i + 1]) 
                        || IsNumericalCharacter(code[i + 1]) 
                        || code[i + 1] == '_'))
                    {
                        word += code[++i];
                    }

                    // Si le mot est un mot cle du langage, on ajout le token correspondant, si non on ajoute un ident
                    if (Token.KeyWordsAssociations.ContainsKey(word))
                        tokens.Add(new Token(Token.KeyWordsAssociations[word], offset));
                    else
                        tokens.Add(new Token(Tokens.Ident, offset) { Ident = word });
                }

                // Si caractere numerique, alors c'est une valeur constante
                else if (IsNumericalCharacter(code[i]))
                {
                    var word = string.Empty;
                    var offset = i;

                    word += code[i];
                    if(i < code.Length - 1 && IsNumericalCharacter(code[i + 1]))
                    {
                        i++;
                        while (i < code.Length && IsNumericalCharacter(code[i]))
                        {
                            //i++;
                            if (i < code.Length && IsAlphabeticalCharacter(code[i]))
                                throw new LexicalException(offset, code[i].ToString());
                            word += code[i];
                            i++;
                        }
                        i--;
                    }
                    else if(i < code.Length - 1 && IsAlphabeticalCharacter(code[i + 1]))
                        throw new LexicalException(offset, code[i].ToString());

                    tokens.Add(new Token(Tokens.Value, offset) { Value = word });
                }

                // Si c'est un caractere special
                else if (IsValidSpecialCharacter(code[i]))
                {
                    var word = string.Empty;
                    var offset = i;
                    
                    word += code[i];
                    // On teste si ce n'est pas une instruction a deux caracteres comme '&&'
                    if (i < code.Length - 1 && IsValidSpecialCharacter(code[i + 1]))
                    {
                        // On cherche tout les caracteres speciaux valides qui sont composes de deux caracteres et qui commencent par le caractere trouve
                        var qDoubleSpecialChars = from specialChar in Token.SpecialCharactersAssociations
                                                  where specialChar.Key.Length == 2 && specialChar.Key[0] == code[i] && specialChar.Key[1] == code[i+1]
                                                  select specialChar.Key;

                        // Si on en a trouve un, on ajoute le deuxieme caractere au mot
                        if (qDoubleSpecialChars.Any())
                            word += code[++i];
                    }

                    if (Token.SpecialCharactersAssociations.ContainsKey(word))
                        tokens.Add(new Token(Token.SpecialCharactersAssociations[word], offset));

                    else
                        throw new LexicalException(offset, word);
                }

                // On passe les espaces, tabulation, etc...
                else if (code[i] == ' ' || code[i] == '\r' || code[i] == '\n' || code[i] == '\t')
                {
                    // Do nothing
                }

                // Si on ne connait pas le caractere, on lance une exception
                else
                    throw new LexicalException(i, code[i].ToString());
            }

            return tokens;
        }

        /// <summary>
        /// Tests whenether a character is a valid special character (defined in Token.SpecialCharacters)
        /// </summary>
        private static bool IsValidSpecialCharacter(char c)
        {
            return Token.SpecialCharacters.Contains(c);
        }

        /// <summary>
        /// Teste si un caractere est une lettre minuscule ou majuscule
        /// </summary>
        private static bool IsAlphabeticalCharacter(char c)
        {
            return c >= 65 && c <= 90 || c >= 97 && c <= 122;
        }

        /// <summary>
        /// Teste si un caractere est un chiffre
        /// </summary>
        private static bool IsNumericalCharacter(char c)
        {
            return c >= 48 && c <= 57;
        }
    }
}
