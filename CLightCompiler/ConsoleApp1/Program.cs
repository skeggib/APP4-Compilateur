using LexicalAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            LexicalAnalyser analyser = new LexicalAnalyser();
            string code = Encoding.ASCII.GetString(Resource.Code);
            List<Token> list = analyser.Convert(code);
            foreach (Token token in list)
            {
                Console.Write(token.ToCode() + " ");
            }
            Console.ReadKey();
        }
    }
}
