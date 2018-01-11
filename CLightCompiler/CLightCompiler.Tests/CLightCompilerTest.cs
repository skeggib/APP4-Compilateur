using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Text;
using LexicalAnalysis;
using SyntaxAnalysis;
using SemanticAnalysis;
using CodeGeneration;

namespace CLightCompiler.Tests
{
    [TestClass]
    public class CLightCompilerTest
    {
        [TestMethod]
        public void FonctionSomme()
        {
            string code = "int main(){out 0;}";
            string result = TestHelper.Run(TestHelper.Compile(code));
            Assert.AreEqual("0", result);
        }

    }
}
