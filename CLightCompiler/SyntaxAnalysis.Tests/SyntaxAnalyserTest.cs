﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using LexicalAnalysis;

namespace SyntaxAnalysis.Tests
{
    [TestClass]
    public class SyntaxAnalyserTest
    {
        private Token _tokenDefault = new Token(Tokens.Void, 0);

        [TestMethod]
        public void SyntaxAnalyserArithmeticsAndLogic()
        {
            const string clCode = @"
int main() {
    a = !2 && 3 < 5 || 1 == (2 + 3 != 5);
}
";

            var tokens = new LexicalAnalyser().Convert(clCode);
            var actualTree = new SyntaxAnalyser().Convert(tokens);

            var expectedTree =
            new Node(Nodes.Program, _tokenDefault,
                new Node(Nodes.DeclFunc, tokens[1],
                    new Node(Nodes.Block, _tokenDefault,
                        new Node(Nodes.Assign, tokens[5],
                            new Node(Nodes.Or, _tokenDefault,
                                new Node(Nodes.And, _tokenDefault,
                                    new Node(Nodes.Not, _tokenDefault,
                                        new Node(Nodes.Const, tokens[8])
                                    ),
                                    new Node(Nodes.LowerThan, _tokenDefault,
                                        new Node(Nodes.Const, tokens[10]),
                                        new Node(Nodes.Const, tokens[12])
                                    )
                                ),
                                new Node(Nodes.AreEqual, _tokenDefault,
                                    new Node(Nodes.Const, tokens[14]),
                                    new Node(Nodes.AreNotEqual, _tokenDefault,
                                        new Node(Nodes.Addition, _tokenDefault,
                                            new Node(Nodes.Const, tokens[17]),
                                            new Node(Nodes.Const, tokens[19])
                                        ),
                                        new Node(Nodes.Const, tokens[21])
                                    )
                                )
                            )
                        )
                    )
                )
            );

            Assert.AreEqual(expectedTree, actualTree);
        }

        [TestMethod]
        public void SyntaxAnalyserDeclarationAndAssignments()
        {
            const string clCode = @"
int main() {
    int i;
    i = 0;
}
";

            var tokens = new LexicalAnalyser().Convert(clCode);
            var actualTree = new SyntaxAnalyser().Convert(tokens);

            var expectedTree =
                new Node(Nodes.Program, _tokenDefault,
                    new Node(Nodes.DeclFunc, tokens[1],
                        new Node(Nodes.Block, _tokenDefault,
                            new Node(Nodes.DeclVar, tokens[6]),
                            new Node(Nodes.Assign, tokens[8],
                                new Node(Nodes.Const, tokens[10])
                            )
                        )));

            Assert.AreEqual(expectedTree, actualTree);
        }

        [TestMethod]
        public void SyntaxAnalyserIfElse()
        {
            const string clCode = @"
int main() {
    int i;
    i = 0;
    if (i == 0)
        i = 1;
    else
    {
        int j;
        j = 1;
        i = i - j;
    }
}
";

            var tokens = new LexicalAnalyser().Convert(clCode);
            var actualTree = new SyntaxAnalyser().Convert(tokens);

            var expectedTree =
                new Node(Nodes.Program, _tokenDefault,
                    new Node(Nodes.DeclFunc, tokens[1],
                        new Node(Nodes.Block, _tokenDefault,
                            new Node(Nodes.DeclVar, tokens[6]),
                            new Node(Nodes.Assign, tokens[8],
                                new Node(Nodes.Const, tokens[10])
                            ),
                            new Node(Nodes.Condition, _tokenDefault,
                                new Node(Nodes.AreEqual, _tokenDefault,
                                    new Node(Nodes.RefVar, tokens[14]),
                                    new Node(Nodes.Const, tokens[16])
                                ),
                                new Node(Nodes.Assign, tokens[18],
                                    new Node(Nodes.Const, tokens[20])
                                ),
                                new Node(Nodes.Block, _tokenDefault,
                                    new Node(Nodes.DeclVar, tokens[25]),
                                    new Node(Nodes.Assign, tokens[27],
                                        new Node(Nodes.Const, tokens[29])
                                    ),
                                    new Node(Nodes.Assign, tokens[31],
                                        new Node(Nodes.Substraction, _tokenDefault,
                                            new Node(Nodes.RefVar, tokens[33]),
                                            new Node(Nodes.RefVar, tokens[35])
                                        )
                                    )
                                )
                            )
                        )));

            Assert.AreEqual(expectedTree, actualTree);
        }

        [TestMethod]
        public void SyntaxAnalyserWhile()
        {
            const string clCode = @"
int main() {
    int i;
    i = 0;
    while (i < 10)
    {
        out i;
        i = i + 1;
    }
}
";

            var tokens = new LexicalAnalyser().Convert(clCode);
            var actualTree = new SyntaxAnalyser().Convert(tokens);

            var expectedTree =
                new Node(Nodes.Program, _tokenDefault,
                    new Node(Nodes.DeclFunc, tokens[1],
            new Node(Nodes.Block, _tokenDefault,
                new Node(Nodes.DeclVar, tokens[6]),
                new Node(Nodes.Assign, tokens[8], new Node(Nodes.Const, tokens[10])),
                new Node(Nodes.Loop, _tokenDefault, new Node(Nodes.Condition, _tokenDefault,
                    new Node(Nodes.LowerThan, _tokenDefault, new Node(Nodes.RefVar, tokens[14]), new Node(Nodes.Const, tokens[16])),
                    new Node(Nodes.Block, _tokenDefault,
                        new Node(Nodes.Out, _tokenDefault, new Node(Nodes.RefVar, tokens[20])),
                        new Node(Nodes.Assign, tokens[22], new Node(Nodes.Addition, _tokenDefault,
                            new Node(Nodes.RefVar, tokens[24]),
                            new Node(Nodes.Const, tokens[26])
                        ))
                    ),
                    new Node(Nodes.Break, _tokenDefault)
                ))
            )));

            Assert.AreEqual(expectedTree, actualTree);
        }

        [TestMethod]
        public void SyntaxAnalyserFor()
        {
            const string clCode = @"
int main() {
    int i;
    for (i = 0; i < 10; i = i + 1) {
        out i;
    }
}
";

            var tokens = new LexicalAnalyser().Convert(clCode);
            var actualTree = new SyntaxAnalyser().Convert(tokens);

            var expectedTree = new Node(Nodes.Program, _tokenDefault,
                new Node(Nodes.DeclFunc, tokens[1],
                    new Node(Nodes.Block, _tokenDefault,
                        new Node(Nodes.DeclVar, tokens[6]),
                        new Node(Nodes.Block, _tokenDefault,
                            new Node(Nodes.Assign, tokens[10], new Node(Nodes.Const, tokens[12])),
                            new Node(Nodes.Loop, _tokenDefault,
                                new Node(Nodes.Condition, _tokenDefault,
                                    new Node(Nodes.LowerThan, _tokenDefault,
                                        new Node(Nodes.RefVar, tokens[14]),
                                        new Node(Nodes.Const, tokens[16])
                                    ),
                                    new Node(Nodes.Block, _tokenDefault,
                                        new Node(Nodes.Block, _tokenDefault,
                                            new Node(Nodes.Out, _tokenDefault, new Node(Nodes.RefVar, tokens[26]))
                                        ),
                                        new Node(Nodes.Assign, tokens[18],
                                            new Node(Nodes.Addition, _tokenDefault,
                                                new Node(Nodes.RefVar, tokens[20]),
                                                new Node(Nodes.Const, tokens[22])
                                            )
                                        )
                                    ),
                                    new Node(Nodes.Break, _tokenDefault)
                                )
                            )
                        )
                    )
                )
            );

            Assert.AreEqual(expectedTree, actualTree);
        }

        [TestMethod]
        public void SyntaxAnalyserCall()
        {
            const string clCode = @"
int main() {
    toto(1, 2);
}
";

            var tokens = new LexicalAnalyser().Convert(clCode);
            var actualTree = new SyntaxAnalyser().Convert(tokens);

            var expectedTree = new Node(Nodes.Program, _tokenDefault,
                new Node(Nodes.DeclFunc, tokens[1],
                    new Node(Nodes.Block, _tokenDefault,
                        new Node(Nodes.Drop, _tokenDefault,
                            new Node(Nodes.Call, tokens[5],
                                new Node(Nodes.Const, tokens[7]),
                                new Node(Nodes.Const, tokens[9])
                            )
                        )
                    )
                )
            );

            Assert.AreEqual(expectedTree, actualTree);
        }

        [TestMethod]
        public void SyntaxAnalyserDeclareFunctionIntNoParams()
        {
            const string clCode = @"
int main() {
    out 3 + 5;
}
";
            var tokens = new LexicalAnalyser().Convert(clCode);
            var actualTree = new SyntaxAnalyser().Convert(tokens);

            var expectedTree = new Node(Nodes.Program, _tokenDefault,
                new Node(Nodes.DeclFunc, tokens[1],
                    new Node(Nodes.Block, _tokenDefault,
                        new Node(Nodes.Out, _tokenDefault,
                            new Node(Nodes.Addition, _tokenDefault,
                                new Node(Nodes.Const, tokens[6]),
                                new Node(Nodes.Const, tokens[8])
                            )
                        )
                    )
                )
            );

            Assert.AreEqual(expectedTree, actualTree);
        }

        [TestMethod]
        public void SyntaxAnalyserDeclareFunctionIntWithParams()
        {
            const string clCode = @"
int main(int a) {
    out a + 5;
}
";
            var tokens = new LexicalAnalyser().Convert(clCode);
            var actualTree = new SyntaxAnalyser().Convert(tokens);

            var expectedTree = new Node(Nodes.Program, _tokenDefault,
                new Node(Nodes.DeclFunc, tokens[1],
                    new Node(Nodes.Block, _tokenDefault,
                        new Node(Nodes.Out, _tokenDefault,
                            new Node(Nodes.Addition, _tokenDefault,
                                new Node(Nodes.RefVar, tokens[8]),
                                new Node(Nodes.Const, tokens[10])
                            )
                        )
                    )
                )
                { Tokens = { tokens[4] } }
            );

            Assert.AreEqual(expectedTree, actualTree);
        }
    }
}
