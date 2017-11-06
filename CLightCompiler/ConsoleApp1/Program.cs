using LexicalAnalysis;
using Symbols;
using SyntaxAnalysis;
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
            Node nConst1 = new Node(NodeCategory.NodeConst, "3");
            Node nConst2 = new Node(NodeCategory.NodeConst, "5");
            Node nAdd = new Node(NodeCategory.NodeAddition, null, nConst1, nConst2);

            Console.WriteLine(nAdd);
            Console.ReadKey();
        }
    }
}
