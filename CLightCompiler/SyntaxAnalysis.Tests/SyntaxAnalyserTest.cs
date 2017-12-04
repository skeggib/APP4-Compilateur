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

            List<Token> tokens = new LexicalAnalyser().Convert(clCode);
            Node actualTree = new SyntaxAnalyser().Convert(tokens);

            Node expectedTree =
            new Node(Nodes.Drop, null,
                new Node(Nodes.Or, null,
                    new Node(Nodes.And, null,
                        new Node(Nodes.Not, null,
                            new Node(Nodes.Const, tokens[1])
                        ),
                        new Node(Nodes.LowerThan, null,
                            new Node(Nodes.Const, tokens[3]),
                            new Node(Nodes.Const, tokens[5])
                        )
                    ),
                    new Node(Nodes.AreEqual, null,
                        new Node(Nodes.Const, tokens[7]),
                        new Node(Nodes.AreNotEqual, null,
                            new Node(Nodes.Addition, null,
                                new Node(Nodes.Const, tokens[10]),
                                new Node(Nodes.Const, tokens[12])
                            ),
                            new Node(Nodes.Const, tokens[14])
                        )
                    )
                )
            );

            Assert.AreEqual(expectedTree, actualTree);
        }

        [TestMethod]
        public void SyntaxAnalyserDeclarationAndAssignments()
        {
            string clCode = @"
{
    int i;
    i = 0;
}
";

            List<Token> tokens = new LexicalAnalyser().Convert(clCode);
            Node actualTree = new SyntaxAnalyser().Convert(tokens);

            Node expectedTree =
            new Node(Nodes.Block, null,
                new Node(Nodes.DeclVar, tokens[2]),
                new Node(Nodes.Assign, tokens[4],
                    new Node(Nodes.Const, tokens[6])
                )
            );

            Assert.AreEqual(expectedTree, actualTree);
        }

        [TestMethod]
        public void SyntaxAnalyserIfElse()
        {
            string clCode = @"
{
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

            List<Token> tokens = new LexicalAnalyser().Convert(clCode);
            Node actualTree = new SyntaxAnalyser().Convert(tokens);

            Node expectedTree =
            new Node(Nodes.Block, null,
                new Node(Nodes.DeclVar, tokens[2]),
                new Node(Nodes.Assign, tokens[4],
                    new Node(Nodes.Const, tokens[6])
                ),
                new Node(Nodes.Condition, null,
                    new Node(Nodes.AreEqual, null,
                        new Node(Nodes.RefVar, tokens[10]),
                        new Node(Nodes.Const, tokens[12])
                    ),
                    new Node(Nodes.Assign, tokens[14],
                        new Node(Nodes.Const, tokens[16])
                    ),
                    new Node(Nodes.Block, null,
                        new Node(Nodes.DeclVar, tokens[21]),
                        new Node(Nodes.Assign, tokens[23],
                            new Node(Nodes.Const, tokens[25])
                        ),
                        new Node(Nodes.Assign, tokens[27],
                            new Node(Nodes.Substraction, null,
                                new Node(Nodes.RefVar, tokens[29]),
                                new Node(Nodes.RefVar, tokens[31])
                            )
                        )
                    )
                )
            );

            Assert.AreEqual(expectedTree, actualTree);
        }

        [TestMethod]
        public void SyntaxAnalyserWhile()
        {
            string clCode = @"
{
    int i;
    i = 0;
    while (i < 10)
    {
        out i;
        i = i + 1;
    }
}
";

            List<Token> tokens = new LexicalAnalyser().Convert(clCode);
            Node actualTree = new SyntaxAnalyser().Convert(tokens);

            Node expectedTree =
            new Node(Nodes.Block, null,
                new Node(Nodes.DeclVar, tokens[2]),
                new Node(Nodes.Assign, tokens[4], new Node(Nodes.Const, tokens[6])),
                new Node(Nodes.Loop, null, new Node(Nodes.Condition, null,
                    new Node(Nodes.LowerThan, null, new Node(Nodes.RefVar, tokens[10]), new Node(Nodes.Const, tokens[12])),
                    new Node(Nodes.Block, null,
                        new Node(Nodes.Out, null, new Node(Nodes.RefVar, tokens[16])),
                        new Node(Nodes.Assign, tokens[18], new Node(Nodes.Addition, null,
                            new Node(Nodes.RefVar, tokens[20]),
                            new Node(Nodes.Const, tokens[22])
                        ))
                    ),
                    new Node(Nodes.Break)
                ))
            );

            Assert.AreEqual(expectedTree, actualTree);
        }

        [TestMethod]
        public void SyntaxAnalyserFor()
        {
            string clCode = @"
{
    int i;
    for (i = 0; i < 10; i = i + 1) {
        out i;
    }
}
";

            List<Token> tokens = new LexicalAnalyser().Convert(clCode);
            Node actualTree = new SyntaxAnalyser().Convert(tokens);

            Node expectedTree =
            new Node(Nodes.Block, null,
                new Node(Nodes.DeclVar, tokens[2]),
                new Node(Nodes.Block, null,
                    new Node(Nodes.Assign, tokens[6], new Node(Nodes.Const, tokens[8])),
                    new Node(Nodes.Loop, null,
                        new Node(Nodes.Condition, null,
                            new Node(Nodes.LowerThan, null,
                                new Node(Nodes.RefVar, tokens[10]),
                                new Node(Nodes.Const, tokens[12])
                            ),
                            new Node(Nodes.Block, null,
                                new Node(Nodes.Block, null,
                                    new Node(Nodes.Out, null, new Node(Nodes.RefVar, tokens[22]))
                                ),
                                new Node(Nodes.Assign, tokens[14],
                                    new Node(Nodes.Addition, null,
                                        new Node(Nodes.RefVar, tokens[16]),
                                        new Node(Nodes.Const, tokens[18])
                                    )
                                )
                            ),
                            new Node(Nodes.Break)
                        )
                    )
                )
            );

            Assert.AreEqual(expectedTree, actualTree);
        }

        [TestMethod]
        public void SyntaxAnalyserCall()
        {
            string clCode = @"
{
    toto(1, 2);
}
";

            List<Token> tokens = new LexicalAnalyser().Convert(clCode);
            Node actualTree = new SyntaxAnalyser().Convert(tokens);

            Node expectedTree = new Node(Nodes.Block, null, 
                new Node(Nodes.Drop, null,
                    new Node(Nodes.Call, tokens[1],
                        new Node(Nodes.Const, tokens[3]),
                        new Node(Nodes.Const, tokens[5]))));

            Assert.AreEqual(expectedTree, actualTree);
        }
    }
}
