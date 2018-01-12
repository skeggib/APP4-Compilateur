using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CLightCompiler.Tests
{
    [TestClass]
    public class CLightCompilerFileTests
    {
        [TestMethod]
        public void TestsProfPass()
        {
            var files = Directory.GetFiles("Tests/Prof/pass", "*.txt");
            foreach (var file in files)
            {
                var output = TestHelper.Run(TestHelper.Compile(File.ReadAllText(file)));
                var outPath = Path.ChangeExtension(file, "out");
                Assert.AreEqual(File.ReadAllText(outPath), output, file);
            }
        }

        [TestMethod]
        public void TestsProfFail()
        {
            var files = Directory.GetFiles("Tests/Prof/fail", "*.txt");
            foreach (var file in files)
            {
                try
                {
                    TestHelper.Compile(File.ReadAllText(file));
                    Assert.Fail(file);
                }
                catch (Exception)
                {
                    
                }
            }
        }

        [TestMethod]
        public void TestsPersoPass()
        {
            var files = Directory.GetFiles("Tests/Perso/pass", "*.c");
            foreach (var file in files)
            {
                var output = TestHelper.Run(TestHelper.Compile(File.ReadAllText(file)));
                var outPath = Path.ChangeExtension(file, "out");
                Assert.AreEqual(File.ReadAllText(outPath), output, file);
            }
        }
    }
}
