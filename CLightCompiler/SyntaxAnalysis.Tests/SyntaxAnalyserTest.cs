using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using LexicalAnalysis;

namespace SyntaxAnalysis.Tests
{
    [TestClass]
    public class SyntaxAnalyserTest
    {
        [TestMethod]
        public void SyntaxAnalyserArithmeticsAndLogic()
        {
            string clCode = "!2 && 3 < 5 || 1 == (2 + 3 != 5);";

            Node expectedTree = 
            new Node(Nodes.Or, null,
                new Node(Nodes.And, null,
                    new Node(Nodes.Not, null,
                        new Node(Nodes.Const, new Token(Tokens.Value, 1) { Value = "2" })
                    ),
                    new Node(Nodes.LowerThan, null,
                        new Node(Nodes.Const, new Token(Tokens.Value, 3) { Value = "3" }),
                        new Node(Nodes.Const, new Token(Tokens.Value, 5) { Value = "5" })
                    )
                ),
                new Node(Nodes.AreEqual, null,
                    new Node(Nodes.Const, new Token(Tokens.Value, 7) { Value = "1" }),
                    new Node(Nodes.AreNotEqual, null,
                        new Node(Nodes.Addition, null,
                            new Node(Nodes.Const, new Token(Tokens.Value, 10) { Value = "2" }),
                            new Node(Nodes.Const, new Token(Tokens.Value, 12) { Value = "3" })
                        ),
                        new Node(Nodes.Const, new Token(Tokens.Value, 14) { Value = "5" })
                    )
                )
            );

            Node actualTree = new SyntaxAnalyser().Convert(new LexicalAnalyser().Convert(clCode));

            Assert.AreEqual(expectedTree, actualTree);
        }
    }
}
