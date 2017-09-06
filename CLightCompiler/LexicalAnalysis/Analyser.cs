using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace LexicalAnalysis
{
    public class Analyser
    {
        private Dictionary<string, TokenCategory> _associations;

        public Analyser()
        {
            _associations = new Dictionary<string, TokenCategory>();
            _associations.Add("if", TokenCategory.TokIf);
        }

        public List<Token> Convert(string code)
        {
            List<Token> tokens = new List<Token>();
            
            string word = string.Empty;

            for (int i = 0; i < code.Length; i++)
            {
                char curr = code[i];

                // Si debut de mot cle ou de variable
                if (IsAlphabeticalCharacter(curr) || curr == '_')
                {
                    int offset = i;

                    // On lit le mot
                    word += curr;
                    while (i < code.Length - 1 && (IsAlphabeticalCharacter(code[i+1]) || IsNumericalCharacter(code[i + 1]) || code[i + 1] == '_'))
                    {
                        i++;
                        word += curr;
                    }

                    // Si mot cle
                    if (_associations.ContainsKey(word))
                    {
                        tokens.Add(new Token(_associations[word], offset));
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

                else if (IsValidSpecialCharacter(curr))
                {
                    
                }

                else if (curr == ' ' || curr == '\r' || curr == '\n' || curr == '\t')
                {
                    // Do nothing
                }

                else
                {
                    throw new LexicalException(i);
                }
            }

            return tokens;
        }

        private bool IsValidSpecialCharacter(char currentChar)
        {
            throw new NotImplementedException();
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
