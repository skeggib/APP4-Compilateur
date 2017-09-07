using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LexicalAnalysis
{
    public class LexicalAnalyser
    {
        private HashSet<char> _specialCharacters;

        public LexicalAnalyser()
        {
            _specialCharacters = new HashSet<char> { '(', ')', '{', '}', '[', ']', ';', '=', '+', '-', '*', '/', '%', '!', '<', '>', '&', '|', '@', ',' };
        }

        public List<Token> Convert(string code)
        {
            List<Token> tokens = new List<Token>();

            for (int i = 0; i < code.Length; i++)
            {
                // Si debut de mot cle ou de variable
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
                        tokens.Add(new Token(TokenCategory.TokIdent, offset)
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
                    while (i < code.Length - 1 && IsNumericalCharacter(code[i + 1]))
                    {
                        i++;
                        word += code[i];
                    }

                    tokens.Add(new Token(TokenCategory.TokValue, offset) { Value = word });
                }

                else if (IsValidSpecialCharacter(code[i]))
                {
                    string word = string.Empty;
                    int offset = i;

                    word += code[i];
                    if (i < code.Length - 1 && IsValidSpecialCharacter(code[i + 1]))
                    {
                        var qDoubleSpecialChars = from specialChar in Token.SpecialCharactersAssociations
                                                  where specialChar.Key.Count() == 2 && specialChar.Key[0] == code[i]
                                                  select specialChar.Key;

                        if (qDoubleSpecialChars.Count() > 0)
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
            return _specialCharacters.Contains(c);
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
