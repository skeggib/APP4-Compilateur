using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace SyntaxAnalysis.Tests
{
    [TestClass]
    public class NodeTest
    {
        [TestMethod]
        public void NodeConstructorCategory()
        {
            Node n = new Node(NodeCategory.NodeAddition);
            Assert.AreEqual(NodeCategory.NodeAddition, n.Category);
        }

        [TestMethod]
        public void NodeConstructorChilds()
        {
            Node n = new Node(NodeCategory.NodeAddition, null,
                new Node(NodeCategory.NodeAddition),
                new Node(NodeCategory.NodeSubstraction));

            Assert.AreEqual(2, n.Childs.Count);
            Assert.AreEqual(NodeCategory.NodeAddition, n.Childs[0].Category);
            Assert.AreEqual(NodeCategory.NodeSubstraction, n.Childs[1].Category);
        }

        [TestMethod]
        public void NodeConstructorConstCategory()
        {
            try
            {
                Node n = new Node(NodeCategory.NodeConst);
                Assert.Fail("On ne devrait pas pouvoir creer de noeud const avec une valeur nulle");
            }
            catch (ArgumentException) { }
        }

        [TestMethod]
        public void NodeConstructorRefVarCategory()
        {
            try
            {
                Node n = new Node(NodeCategory.NodeRefVar);
                Assert.Fail("On ne devrait pas pouvoir creer de noeud reference avec une valeur nulle");
            }
            catch (ArgumentException) { }
        }

        [TestMethod]
        public void NodeConstructorRefFuncCategory()
        {
            try
            {
                Node n = new Node(NodeCategory.NodeRefFunc);
                Assert.Fail("On ne devrait pas pouvoir creer de noeud reference avec une valeur nulle");
            }
            catch (ArgumentException) { }
        }
    }
}
