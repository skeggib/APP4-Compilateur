using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;

namespace LexicalAnalysis.Tests
{
    [TestClass]
    public class LexicalAnalyserTest
    {
        private string ReadFile(bool valid, string filename)
        {
            StreamReader reader = new StreamReader("../../CLightCode/" + (valid ? "Valid" : "Invalid") + "/" + filename + ".cl");
            return reader.ReadToEnd();
        }

        private string CleanCode(string code)
        {
            return code
                .Replace(" ", string.Empty)
                .Replace("\n", string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\t", string.Empty);
        }

        private void TestFile(bool valid, string filename)
        {
            LexicalAnalyser analizer = new LexicalAnalyser();

            string code = ReadFile(valid, filename);
            try
            {
                List<Token> tokens = analizer.Convert(code);
                string expected = CleanCode(code);

                string given = string.Empty;
                foreach (Token token in tokens)
                {
                    given += token.ToCode();
                }

                Assert.AreEqual(expected, given, filename + "(" + (valid ? "valid" : "invalid") + ")" + " failed");
            }
            catch (LexicalException e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void LexicalAnaliserKeywords()
        {
            TestFile(true, "keywords");
        }

        [TestMethod]
        public void LexicalAnaliserMain()
        {
            TestFile(true, "main");
        }

        [TestMethod]
        public void LexicalAnaliserSpecialCharacters()
        {
            TestFile(true, "specialCharacters");
        }
    }
}
