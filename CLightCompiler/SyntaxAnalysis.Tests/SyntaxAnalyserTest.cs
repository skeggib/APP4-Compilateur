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
            string clCode = "!2 && 3 < 5 || 1 == (2 + 3 != 5)";

            Node expectedTree = 
            new Node(NodeCategory.NodeLogicOr, null,
                new Node(NodeCategory.NodeLogicAnd, null,
                    new Node(NodeCategory.NodeLogicNot, null,
                        new Node(NodeCategory.NodeConst, "2")
                    ),
                    new Node(NodeCategory.NodeLessThan, null,
                        new Node(NodeCategory.NodeConst, "3"),
                        new Node(NodeCategory.NodeConst, "5")
                    )
                ),
                new Node(NodeCategory.NodeAreEqual, null,
                    new Node(NodeCategory.NodeConst, "1"),
                    new Node(NodeCategory.NodeAreNotEqual, null,
                        new Node(NodeCategory.NodeAddition, null,
                            new Node(NodeCategory.NodeConst, "2"),
                            new Node(NodeCategory.NodeConst, "3")
                        ),
                        new Node(NodeCategory.NodeConst, "5")
                    )
                )
            );

            Node actualTree = new SyntaxAnalyser().Convert(new LexicalAnalyser().Convert(clCode));

            Assert.AreEqual(expectedTree, actualTree);
        }
    }
}
