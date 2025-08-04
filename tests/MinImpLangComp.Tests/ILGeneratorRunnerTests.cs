using MinImpLangComp.AST;
using MinImpLangComp.Lexing;
using MinImpLangComp.ILGeneration;
using System.Globalization;

namespace MinImpLangComp.Tests
{
    public class ILGeneratorRunnerTests
    {
        [Fact]
        public void Should_Evaluate_IntegerLiteral_Correctly()
        {
            var expr = new IntegerLiteral(42);
            var result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.Equal(42, result);
        }

        [Fact]
        public void Should_Evaluate_FloatLiteral_Correctly()
        {
            var expr = new FloatLiteral(3.14);
            var result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.True(result is double d && d >= 3.139 && d <= 3.141);
        }

        [Fact]
        public void Should_Evaluate_Plus_Correctly()
        {
            var expr = new BinaryExpression(
                new IntegerLiteral(7),
                OperatorType.Plus,
                new IntegerLiteral(5)
            );
            var result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.Equal(12, result);
        }

        [Fact]
        public void Should_Evaluate_Minus_Correctly()
        {
            var expr = new BinaryExpression(
                new IntegerLiteral(10),
                OperatorType.Minus,
                new IntegerLiteral(4)
            );
            var result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.Equal(6, result);
        }

        [Fact]
        public void Should_Evaluate_Multiply_Correctly()
        {
            var expr = new BinaryExpression(
                new IntegerLiteral(3),
                OperatorType.Multiply,
                new IntegerLiteral(6)
            );
            var result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.Equal(18, result);
        }

        [Fact]
        public void Should_Evaluate_Divide_Correctly()
        {
            var expr = new BinaryExpression(
                new IntegerLiteral(12),
                OperatorType.Divide,
                new IntegerLiteral(3)
            );
            var result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.Equal(4, result);
        }

        [Fact]
        public void Should_Evaluate_Module_Correctly()
        {
            var expr = new BinaryExpression(
                new IntegerLiteral(10),
                OperatorType.Modulo,
                new IntegerLiteral(3)
            );
            var result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.Equal(1, result);
        }

        [Fact]
        public void Should_Evaluate_EqualEqual_Correctly()
        {
            var expr = new BinaryExpression(
                new IntegerLiteral(7),
                OperatorType.Equalequal,
                new IntegerLiteral(7)
            );
            var result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.Equal(1, result);

            expr = new BinaryExpression(
                new IntegerLiteral(7),
                OperatorType.Equalequal,
                new IntegerLiteral(5)
            );
            result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.Equal(0, result);
        }

        [Fact]
        public void Should_Evaluate_NotEqual_Correctly()
        {
            var expr = new BinaryExpression(
                new IntegerLiteral(5),
                OperatorType.NotEqual,
                new IntegerLiteral(3)
            );
            var result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.Equal(1, result);
        }

        [Fact]
        public void Should_Evaluate_Less_Correctly()
        {
            var expr = new BinaryExpression(
                new IntegerLiteral(2),
                OperatorType.Less,
                new IntegerLiteral(10)
            );
            var result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.Equal(1, result);
        }

        [Fact]
        public void Should_Evaluate_LessEqual_Correctly()
        {
            var expr = new BinaryExpression(
                new IntegerLiteral(7),
                OperatorType.LessEqual,
                new IntegerLiteral(7)
            );
            var result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.Equal(1, result);
        }

        [Fact]
        public void Should_Evaluate_Greater_Correctly()
        {
            var expr = new BinaryExpression(
                new IntegerLiteral(10),
                OperatorType.Greater,
                new IntegerLiteral(3)
            );
            var result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.Equal(1, result);
        }

        [Fact]
        public void Should_Evaluate_GreaterEqual_Correctly()
        {
            var expr = new BinaryExpression(
                new IntegerLiteral(8),
                OperatorType.GreaterEqual,
                new IntegerLiteral(8)
            );
            var result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.Equal(1, result);
        }

        [Fact]
        public void Should_Evaluate_BitiwseAnd_Correctly()
        {
            var expr = new BinaryExpression(
                new IntegerLiteral(6), // 110
                OperatorType.BitwiseAnd,
                new IntegerLiteral(3) // 011
            );
            var result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.Equal(2, result); // 110 & 011 = 010
        }

        [Fact]
        public void Should_Evaluate_BitwiseOr_Correctly()
        {
            var expr = new BinaryExpression(
                new IntegerLiteral(6), // 110
                OperatorType.BitwiseOr,
                new IntegerLiteral(3) // 011
            );
            var result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.Equal(7, result); // 110 | 011 = 111
        }

        [Fact]
        public void Should_Evaluate_LogicalAnd_Correctly()
        {
            var expr = new BinaryExpression(
                new IntegerLiteral(1),
                OperatorType.AndAnd,
                new IntegerLiteral(0)
            );
            var result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.Equal(0, result);

            expr = new BinaryExpression(
                new IntegerLiteral(1),
                OperatorType.AndAnd,
                new IntegerLiteral(1)
            );
            result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.Equal(1, result);
        }

        [Fact]
        public void Should_Evaluate_LogicalOr_Correctly()
        {
            var expr = new BinaryExpression(
                new IntegerLiteral(0),
                OperatorType.OrOr,
                new IntegerLiteral(0)
            );
            var result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.Equal(0, result);

            expr = new BinaryExpression(
                new IntegerLiteral(0),
                OperatorType.OrOr,
                new IntegerLiteral(1)
            );
            result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.Equal(1, result);
        }

        [Fact]
        public void Should_Respect_OperatorPrecedence()
        {
            var expr = new BinaryExpression(
                new IntegerLiteral(1),
                OperatorType.Plus,
                new BinaryExpression(
                    new IntegerLiteral(2),
                    OperatorType.Multiply,
                    new IntegerLiteral(3)
                )
            );
            var result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.Equal(7, result);
        }

        [Fact]
        public void Should_Respect_ParenthesizedGrouping()
        {
            var expr = new BinaryExpression(
                new BinaryExpression(
                    new IntegerLiteral(1),
                    OperatorType.Plus,
                    new IntegerLiteral(2)
                ),
                OperatorType.Multiply,
                new IntegerLiteral(3)
            );
            var result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.Equal(9, result);
        }

        [Fact]
        public void Should_Respect_LeftAssociativity()
        {
            var expr = new BinaryExpression(
                new BinaryExpression(
                    new IntegerLiteral(10),
                    OperatorType.Minus,
                    new IntegerLiteral(3)
                ),
                OperatorType.Minus,
                new IntegerLiteral(2)
            );
            var result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.Equal(5, result);
        }

        [Fact]
        public void Should_Evaluate_LogicalAnd_Complex()
        {
            var expr = new BinaryExpression(
                new BinaryExpression(
                    new IntegerLiteral(5),
                    OperatorType.Greater,
                    new IntegerLiteral(3)
                ),
                OperatorType.AndAnd,
                new BinaryExpression(
                    new IntegerLiteral(2),
                    OperatorType.Less,
                    new IntegerLiteral(4)
                )
            );
            var result = ILGeneratorRunner.GenerateAndRunIL(expr);

            // (5 > 3) && (2 < 4) -> 1 && 1 -> 1
            Assert.Equal(1, result);
        }

        [Fact]
        public void Should_Evaluate_LogicalOr_Complex()
        {
            var expr = new BinaryExpression(
                new BinaryExpression(
                    new IntegerLiteral(5),
                    OperatorType.Greater,
                    new IntegerLiteral(10)
                ),
                OperatorType.OrOr,
                new BinaryExpression(
                    new IntegerLiteral(2),
                    OperatorType.Less,
                    new IntegerLiteral(4)
                )
            );
            var result = ILGeneratorRunner.GenerateAndRunIL(expr);

            // (5 > 10) || (2 < 4) -> 0 || 1 -> 1
            Assert.Equal(1, result);
        }

        [Fact]
        public void Should_Print_IntegerLiteral()
        {
            var expr = new FunctionCall("print", new List<Expression>
            {
                new IntegerLiteral(123)
            });
            using var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            ILGeneratorRunner.GenerateAndRunIL(expr);
            var outpout = consoleOutput.ToString().Trim();

            Assert.Equal("123", outpout);
        }

        [Fact]
        public void Should_Print_MultipleValues()
        {
            var expr = new FunctionCall("print", new List<Expression>
            {
                new IntegerLiteral(42),
                new FloatLiteral(3.14)
            });
            var originalculture = CultureInfo.CurrentCulture;
            var originalOut = Console.Out;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
                using var consoleOutput = new StringWriter();
                Console.SetOut(consoleOutput);
                ILGeneratorRunner.GenerateAndRunIL(expr);
                var output = consoleOutput.ToString().Trim().Split(Environment.NewLine);

                Assert.Equal("42", output[0]);
                Assert.Equal("3.14", output[1]);
            }
            finally
            {
                CultureInfo.CurrentCulture = originalculture;
                Console.SetOut(originalOut);
            }
        }

        [Fact]
        public void Should_Print_SimpleAddition()
        {
            var expr = new FunctionCall("print", new List<Expression>
            {
                new BinaryExpression(
                    new IntegerLiteral(1),
                    OperatorType.Plus,
                    new IntegerLiteral(2)
                )
            });
            using var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            ILGeneratorRunner.GenerateAndRunIL(expr);
            var output = consoleOutput.ToString().Trim();

            Assert.Equal("3", output);
        }

        [Fact]
        public void Should_Print_FloatDivision()
        {
            var expr = new FunctionCall("print", new List<Expression>
            {
                new BinaryExpression(
                    new IntegerLiteral(10),
                    OperatorType.Divide,
                    new FloatLiteral(4.0)
                )
            });
            var originalculture = CultureInfo.CurrentCulture;
            var originalOut = Console.Out;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
                using var consoleOutput = new StringWriter();
                Console.SetOut(consoleOutput);
                ILGeneratorRunner.GenerateAndRunIL(expr);
                var output = consoleOutput.ToString().Trim();

                Assert.Equal("2.5", output);
            }
            finally
            {
                CultureInfo.CurrentCulture = originalculture;
                Console.SetOut(originalOut);
            }
        }

        [Fact]
        public void Should_Print_ParenthesizedExpression()
        {
            var expr = new FunctionCall("print", new List<Expression>
            {
                new BinaryExpression(
                    new BinaryExpression(
                        new IntegerLiteral(2),
                        OperatorType.Plus,
                        new IntegerLiteral(3)
                    ),
                    OperatorType.Multiply,
                    new IntegerLiteral(4)
                )
            });
            using var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            ILGeneratorRunner.GenerateAndRunIL(expr);
            var output = consoleOutput.ToString().Trim();

            Assert.Equal("20", output);
        }


        /////// ILGeneratorRunner.cs with statements here : //////
        
        private string CaptureConsole(Action action) // Utilitaire
        {
            var originalOut = Console.Out;
            using var writer = new StringWriter();            
            try
            {
                Console.SetOut(writer);
                action();
            }
            finally
            {
                Console.SetOut(originalOut);
            }
            return writer.ToString().Trim();
        }


        [Fact]
        public void Bind_IntegerLiteral_DoesNotThrow()
        {
            var statements = new List<Statement>
            {
                new ConstantDeclaration("x", new IntegerLiteral(123), new TypeAnnotation("int", TokenType.TypeInt))
            };
            var exception = Record.Exception(() => ILGeneratorRunner.GenerateAndRunIL(statements));

            Assert.Null(exception);
        }

        [Fact]
        public void Set_SimpleBinaryAddition_DoesNotThrow()
        {
            var statements = new List<Statement>
            {
                new VariableDeclaration("y", new BinaryExpression(
                        new IntegerLiteral(10),
                        OperatorType.Plus,
                        new IntegerLiteral(5)
                    ),
                    new TypeAnnotation("int", TokenType.TypeInt)
                )
            };
            var exception = Record.Exception(() => ILGeneratorRunner.GenerateAndRunIL(statements));

            Assert.Null(exception);
        }

        [Fact]
        public void Evaluate_SimpleAddition_ReturnsCorrectResult()
        {
            var expr = new BinaryExpression(
                new IntegerLiteral(4),
                OperatorType.Plus,
                new IntegerLiteral(5)
            );
            var result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.Equal(9, result);
        }

        [Fact]
        public void Print_SimpleExpression_PrintsExpectedOutput()
        {
            var statements = new List<Statement>
            {
                new ExpressionStatement(
                    new FunctionCall("print", new List<Expression>
                    {
                        new BinaryExpression(new IntegerLiteral(2), OperatorType.Plus, new IntegerLiteral(3))
                    })
                )
            };
            var output = CaptureConsole (() =>
            {
                ILGeneratorRunner.GenerateAndRunIL(statements);
            });

            Assert.Equal("5", output);            
        }

        [Fact]
        public void Print_IntegerLiteral_PrintsExpectedOutput()
        {
            var statements = new List<Statement>
            {
                new ExpressionStatement(
                    new FunctionCall("print", new List<Expression>
                    {
                        new IntegerLiteral(42)
                    })
                )
            };
            var output = CaptureConsole(() =>
            {
                ILGeneratorRunner.GenerateAndRunIL(statements);
            });

            Assert.Equal("42", output);
        }

        [Fact]
        public void Print_FloatLiteral_PrintsExpectedOutput()
        {
            var statements = new List<Statement>
            {
                new ExpressionStatement(
                    new FunctionCall("print", new List<Expression>
                    {
                        new FloatLiteral(3.14)
                    })
                )
            };
            var originalCulture = CultureInfo.CurrentCulture;
            var originalOut = Console.Out;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
                var output = CaptureConsole(() =>
                {
                    ILGeneratorRunner.GenerateAndRunIL(statements);
                });

                Assert.Equal("3.14", output);
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
                Console.SetOut(originalOut);
            }
        }

        [Fact]
        public void Print_ParenthesizedExpression_PrintsExpectedOutput()
        {
            var statements = new List<Statement>
            {
                new ExpressionStatement(
                    new FunctionCall("print", new List<Expression>
                    {
                        new BinaryExpression(new BinaryExpression(new IntegerLiteral(1), OperatorType.Plus, new IntegerLiteral(2)),
                        OperatorType.Multiply,
                        new IntegerLiteral(2))
                    })
                )
            };
            var output = CaptureConsole(() =>
            {
                ILGeneratorRunner.GenerateAndRunIL(statements);
            });

            Assert.Equal("6", output);
        }

        [Fact]
        public void Assignment_ToDeclareSetVariable_Works()
        {
            var statements = new List<Statement>
            {
                new VariableDeclaration("x", new IntegerLiteral(10), new TypeAnnotation("int", TokenType.TypeInt)),
                new Assignment("x", new IntegerLiteral(42))
            };
            var result = ILGeneratorRunner.GenerateAndRunIL(statements);

            Assert.Equal(42, result);
        }

        [Fact]
        public void Assignment_ToUndeclaredVariable_Throws()
        {
            var statements = new List<Statement>
            {
                new Assignment("x", new IntegerLiteral(5))
            };

            Assert.Throws<InvalidOperationException>(() => ILGeneratorRunner.GenerateAndRunIL(statements));
        }

        [Fact]
        public void Assignment_ToConstantBind_Throws()
        {
            var statements = new List<Statement>
            {
                new ConstantDeclaration("x", new IntegerLiteral(5), new TypeAnnotation("int", TokenType.TypeInt)),
                new Assignment("x", new IntegerLiteral(10))
            };

            Assert.Throws<InvalidOperationException>(() => ILGeneratorRunner.GenerateAndRunIL(statements));
        }

        //////////////////

        [Fact]
        public void IfStatement_BranchTrue_EvaluatesThenBranch()
        {
            var statements = new List<Statement>
            {
                new VariableDeclaration("x", new IntegerLiteral(0), new TypeAnnotation("int", TokenType.TypeInt)),
                new IfStatement
                (
                    new BooleanLiteral(true),
                    new Assignment("x", new IntegerLiteral(10)),
                    new Assignment("x", new IntegerLiteral(20))
                ),
                new ExpressionStatement(new VariableReference("x"))
            };
            var result = ILGeneratorRunner.GenerateAndRunIL(statements);

            Assert.Equal(10, result);
        }

        [Fact]
        public void IfStatement_BranchFalse_EvaluatesElseBranch()
        {
            var statements = new List<Statement>
            {
                new VariableDeclaration("x", new IntegerLiteral(0), new TypeAnnotation("int", TokenType.TypeInt)),
                new IfStatement
                (
                    new BooleanLiteral(false),
                    new Assignment("x", new IntegerLiteral(10)),
                    new Assignment("x", new IntegerLiteral(20))
                ),
                new ExpressionStatement(new VariableReference("x"))
            };
            var result = ILGeneratorRunner.GenerateAndRunIL(statements);

            Assert.Equal(20, result);
        }

        [Fact]
        public void IfStatement_BranchFalse_NoElse_DoesNothing()
        {
            var statement = new List<Statement>
            {
                new VariableDeclaration("x", new IntegerLiteral(5), new TypeAnnotation("int", TokenType.TypeInt)),
                new IfStatement
                (
                    new BooleanLiteral(false),
                    new Assignment("x", new IntegerLiteral(99))
                ),
                new ExpressionStatement(new VariableReference("x"))
            };
            var result = ILGeneratorRunner.GenerateAndRunIL(statement);

            Assert.Equal(5, result);
        }

        [Fact]
        public void IfStatement_BranchTrue_NoElse_ExecutesThen()
        {
            var statements = new List<Statement>
            {
                new VariableDeclaration("x", new IntegerLiteral(1), new TypeAnnotation("int", TokenType.TypeInt)),
                new IfStatement
                (
                    new BooleanLiteral(true),
                    new Assignment("x", new IntegerLiteral(42))
                ),
                new ExpressionStatement(new VariableReference("x"))
            };
            var result = ILGeneratorRunner.GenerateAndRunIL(statements);

            Assert.Equal(42, result);
        }

        [Fact]
        public void IfStatement_WithBinaryCondition_EvaluatesCorrectBranch()
        {
            var condition = new BinaryExpression(new IntegerLiteral(1), OperatorType.Less, new IntegerLiteral(2));
            var statements = new List<Statement>
            {
                new VariableDeclaration("x", new IntegerLiteral(0), new TypeAnnotation("int", TokenType.TypeInt)),
                new IfStatement
                (
                    condition,
                    new Assignment("x", new IntegerLiteral(100)),
                    new Assignment("x", new IntegerLiteral(-1))
                ),
                new ExpressionStatement(new VariableReference("x"))
            };
            var result = ILGeneratorRunner.GenerateAndRunIL(statements);

            Assert.Equal(100, result);
        }

        [Fact]
        public void IfStatement_WithNonBooleanCondition_ThrowsException()
        {
            var statements = new List<Statement>
            {
                new VariableDeclaration("x", new IntegerLiteral(0), new TypeAnnotation("int", TokenType.TypeInt)),
                new IfStatement
                (
                    new IntegerLiteral(1),
                    new Assignment("x", new IntegerLiteral(10)),
                    new Assignment("x", new IntegerLiteral(20))
                )
            };

            var error = Assert.Throws<InvalidOperationException>(() =>
            {
                ILGeneratorRunner.GenerateAndRunIL(statements);
            });
            Assert.Contains("The condition in if-statement must be of type 'bool'", error.Message);
        }

        [Fact]
        public void IfStatement_AssignsToUndeclaredVariable_ThrowsException()
        {
            var statements = new List<Statement>
            {
                new IfStatement
                (
                    new BooleanLiteral(true),
                    new Assignment("Undeclared", new IntegerLiteral(321))
                )
            };

            var error = Assert.Throws<InvalidOperationException>(() =>
            {
                ILGeneratorRunner.GenerateAndRunIL(statements);
            });
            Assert.Contains("not declared", error.Message);
        }
    }
}
