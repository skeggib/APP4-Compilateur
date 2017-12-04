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
            Node n = new Node(Nodes.Addition);
            Assert.AreEqual(Nodes.Addition, n.Category);
        }

        [TestMethod]
        public void NodeConstructorChilds()
        {
            Node n = new Node(Nodes.Addition, null,
                new Node(Nodes.Addition),
                new Node(Nodes.Substraction));

            Assert.AreEqual(2, n.Childs.Count);
            Assert.AreEqual(Nodes.Addition, n.Childs[0].Category);
            Assert.AreEqual(Nodes.Substraction, n.Childs[1].Category);
        }

        [TestMethod]
        public void NodeConstructorConstCategory()
        {
            try
            {
                Node n = new Node(Nodes.Const);
                Assert.Fail("On ne devrait pas pouvoir creer de noeud const avec une valeur nulle");
            }
            catch (ArgumentException) { }
        }

        [TestMethod]
        public void NodeConstructorRefVarCategory()
        {
            try
            {
                Node n = new Node(Nodes.RefVar);
                Assert.Fail("On ne devrait pas pouvoir creer de noeud reference avec une valeur nulle");
            }
            catch (ArgumentException) { }
        }

        [TestMethod]
        public void NodeConstructorRefFuncCategory()
        {
            try
            {
                Node n = new Node(Nodes.Call);
                Assert.Fail("On ne devrait pas pouvoir creer de noeud reference avec une valeur nulle");
            }
            catch (ArgumentException) { }
        }

        [TestMethod]
        public void NodeConstructorAssignCategory()
        {
            try
            {
                Node n = new Node(Nodes.Assign);
                Assert.Fail("On ne devrait pas pouvoir creer de noeud reference avec une valeur nulle");
            }
            catch (ArgumentException) { }
        }

        [TestMethod]
        public void NodeConstructorDeclarationCategory()
        {
            try
            {
                Node n = new Node(Nodes.DeclVar);
                Assert.Fail("On ne devrait pas pouvoir creer de noeud reference avec une valeur nulle");
            }
            catch (ArgumentException) { }
        }
    }
}
