using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;

namespace LexicalAnalysis.Tests
{
    [TestClass]
    public class LexicalAnalyserTest
    {
        private string ReadFile(string folder, string filename)
        {
            StreamReader reader = new StreamReader("../../CLightCode/" + folder + "/" + filename + ".cl");
            return reader.ReadToEnd();
        }

        private void TestFile(bool valid, string filename)
        {
            if (valid)
            {
                LexicalAnalyser analiser = new LexicalAnalyser();
                string code = ReadFile("Valid", filename);
                try
                {
                    List<Token> tokens = analiser.Convert(code);
                    string expected = ReadFile("ValidCorrect", filename);

                    string given = string.Empty;
                    for (int i = 0; i < tokens.Count; i++)
                    {
                        given += (i > 0 ? " " : string.Empty) + tokens[i].ToCode();
                    }

                    Assert.AreEqual(expected, given);
                }
                catch (LexicalException e)
                {
                    Assert.Fail(e.Message);
                }
            }

            else
            {
                LexicalAnalyser analiser = new LexicalAnalyser();
                string code = ReadFile("Invalid", filename);
                try
                {
                    analiser.Convert(code);
                    Assert.Fail("Invalid/" + filename + " should fail");
                }

                catch (LexicalException)
                {

                }
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

        [TestMethod]
        public void LexicalAnaliserUnknownCharacter()
        {
            TestFile(false, "unknownCharacter");
        }
    }
}
