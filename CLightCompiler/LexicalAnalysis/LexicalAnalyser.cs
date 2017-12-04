using System.Collections.Generic;
using System.Linq;

namespace LexicalAnalysis
{
    public class LexicalAnalyser
    {
        public List<Token> Convert(string code)
        {
            var tokens = new List<Token>();

            for (var i = 0; i < code.Length; i++)
            {
                if (IsAlphabeticalCharacter(code[i]) || code[i] == '_')
                {
                    string word = string.Empty;
                    int offset = i;

                    // On lit le mot
                    word += code[i];
                    while (i < code.Length - 1 && (IsAlphabeticalCharacter(code[i + 1]) || IsNumericalCharacter(code[i + 1]) || code[i + 1] == '_'))
                    {
                        i++;
                        word += code[i];
                    }

                    // Si mot cle
                    if (Token.KeyWordsAssociations.ContainsKey(word))
                    {
                        tokens.Add(new Token(Token.KeyWordsAssociations[word], offset));
                    }

                    // Si variable
                    else
                    {
                        tokens.Add(new Token(Tokens.Ident, offset)
                        {
                            Ident = word
                        });
                    }
                }

                else if (IsNumericalCharacter(code[i]))
                {
                    string word = string.Empty;
                    int offset = i;

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

                else if (IsValidSpecialCharacter(code[i]))
                {
                    string word = string.Empty;
                    int offset = i;

                    word += code[i];
                    if (i < code.Length - 1 && IsValidSpecialCharacter(code[i + 1]))
                    {
                        var qDoubleSpecialChars = from specialChar in Token.SpecialCharactersAssociations
                                                  where specialChar.Key.Length == 2 && specialChar.Key[0] == code[i]
                                                  select specialChar.Key;

                        if (qDoubleSpecialChars.Any())
                        {
                            i++;
                            word += code[i];
                        }
                    }

                    if (Token.SpecialCharactersAssociations.ContainsKey(word))
                    {
                        tokens.Add(new Token(Token.SpecialCharactersAssociations[word], offset));
                    }

                    else
                    {
                        throw new LexicalException(offset, word);
                    }
                }

                else if (code[i] == ' ' || code[i] == '\r' || code[i] == '\n' || code[i] == '\t')
                {
                    // Do nothing
                }

                else
                {
                    throw new LexicalException(i, code[i].ToString());
                }
            }

            return tokens;
        }

        private bool IsValidSpecialCharacter(char c)
        {
            return Token.SpecialCharacters.Contains(c);
        }

        private bool IsAlphabeticalCharacter(char c)
        {
            return (c >= 65 && c <= 90) || (c >= 97 && c <= 122);
        }

        private bool IsNumericalCharacter(char c)
        {
            return c >= 48 && c <= 57;
        }
    }
}
