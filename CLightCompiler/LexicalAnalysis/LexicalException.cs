using System;
using System.Collections.Generic;
using System.Text;

namespace LexicalAnalysis
{
    /// <summary>
    /// Exception pouvant etre lancee durant l'analyse lexicale
    /// </summary>
    public class LexicalException : Exception
    {
        /// <summary>
        /// Position du premier caractere responsable dans le fichier source
        /// </summary>
        public int Offset { get; private set; }
        
        /// <summary>
        /// Texte reponsable de l'exception
        /// </summary>
        public string Word { get; private set; }

        public LexicalException(int offset, string word)
            : base("Lexical exception: '" + word + "' at offset " + offset)
        {
            Offset = offset;
            Word = word;
        }
    }
}
