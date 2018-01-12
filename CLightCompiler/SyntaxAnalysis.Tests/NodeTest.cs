using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using LexicalAnalysis;

namespace SyntaxAnalysis.Tests
{
    [TestClass]
    public class NodeTest
    {
        private Token _tokenDefault = new Token(Tokens.Void, 0);

        [TestMethod]
        public void NodeConstructorCategory()
        {
            Node n = new Node(Nodes.Addition, _tokenDefault);
            Assert.AreEqual(Nodes.Addition, n.Category);
        }

        [TestMethod]
        public void NodeConstructorChilds()
        {
            Node n = new Node(Nodes.Addition, _tokenDefault,
                new Node(Nodes.Addition, _tokenDefault),
                new Node(Nodes.Substraction, _tokenDefault));

            Assert.AreEqual(2, n.Childs.Count);
            Assert.AreEqual(Nodes.Addition, n.Childs[0].Category);
            Assert.AreEqual(Nodes.Substraction, n.Childs[1].Category);
        }
    }
}
