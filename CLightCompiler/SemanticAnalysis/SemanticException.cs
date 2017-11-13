using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysis
{
    public class SemanticException : Exception
    {
        /// <summary>
        /// Position du premier caractere responsable dans le fichier source
        /// </summary>
        public int Offset { get; private set; }

        public SemanticException(int offset, string reason)
            : base($"Semantic error: {reason} at offset {offset}")
        {
            Offset = offset;
        }
    }
}
