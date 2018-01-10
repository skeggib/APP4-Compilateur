using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxAnalysis
{
    public class SyntaxException : Exception
    {
        /// <summary>
        /// Position du premier caractere responsable dans le fichier source
        /// </summary>
        public int Offset { get; private set; }

        public SyntaxException(int offset, string reason)
            : base("Syntax error: " + reason + " at offset " + offset)
        {
            Offset = offset;
        }
    }
}
