using MinImpLangComp.AST;
using MinImpLangComp.Lexing;
using MinImpLangComp.ILGeneration;
using System.Globalization;
using Xunit;

namespace MinImpLangComp.Tests
{
    /// <summary>
    /// Tests that validate IL generation and execution behavior for expressions and statements.
    /// </summary>
    [Collection("ConsoleSerial")]
    public class ILGeneratorRunnerTests
    {
        #region issue13
        /// <summary>
        /// 42 should evaluate 42.
        /// </summary>
        [Fact]
        public void Should_Evaluate_IntegerLiteral_Correctly()
        {
            var expr = new IntegerLiteral(42);
            var result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.Equal(42, result);
        }

        /// <summary>
        /// 3.14 should evaluate near 3.14 as double.
        /// </summary>
        [Fact]
        public void Should_Evaluate_FloatLiteral_Correctly()
        {
            var expr = new FloatLiteral(3.14);
            var result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.True(result is double d && d >= 3.139 && d <= 3.141);
        }
        
        /// <summary>
        /// 7 + 5 = 12.
        /// </summary>
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

        /// <summary>
        /// 10 - 4 = 6.
        /// </summary>
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

        /// <summary>
        /// 3 * 6 = 18.
        /// </summary>
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

        /// <summary>
        /// 12 / 3 = 4.
        /// </summary>
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

        /// <summary>
        /// 10 % 3 = 1.
        /// </summary>
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

        /// <summary>
        /// Equality returns 1 for true, 0 for false (int semantics).
        /// </summary>
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

        /// <summary>
        /// Inequality returns 1 for true.
        /// </summary>
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

        /// <summary>
        /// 2 &lt; 10 -> 1.
        /// </summary>
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

        /// <summary>
        /// 7 &lt;= 7 -> 1.
        /// </summary>
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

        /// <summary>
        /// 10 &gt; 3 -> 1.
        /// </summary>
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

        /// <summary>
        /// 8 &gt;= 8 -> 1.
        /// </summary>
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

        /// <summary>
        /// Bitwise AND: 6 &amp; 3 = 2.
        /// </summary>
        [Fact]
        public void Should_Evaluate_BitwiseAnd_Correctly()
        {
            var expr = new BinaryExpression(
                new IntegerLiteral(6), // 110
                OperatorType.BitwiseAnd,
                new IntegerLiteral(3) // 011
            );
            var result = ILGeneratorRunner.GenerateAndRunIL(expr);

            Assert.Equal(2, result); // 110 & 011 = 010
        }

        /// <summary>
        /// Bitwise OR: 6 | 3 = 7.
        /// </summary>
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

        /// <summary>
        /// Logical AND (short-circuit) with integer 0/1 semantics.
        /// </summary>
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

        /// <summary>
        /// Logical OR (short-circuit) with integer 0/1 semantics.
        /// </summary>
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

        /// <summary>
        /// Operator precedence: 1 + 2 * 3 = 7.
        /// </summary>
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

        /// <summary>
        /// Parantheses grouping: (1 + 2) * 3 = 9.
        /// </summary>
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

        /// <summary>
        /// Left associativity : (10 - 3) - 2 = 5.
        /// </summary>
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

        /// <summary>
        /// Complex logical AND expression returns 1.
        /// </summary>
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

        /// <summary>
        /// Complex logical OR expression returns 1.
        /// </summary>
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

        /// <summary>
        /// print(123) writes "123" (capture from Console.Out).
        /// </summary>
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
            var output = consoleOutput.ToString().Trim();

            Assert.Equal("123", output);
        }

        /// <summary>
        /// Print(42, 3.14) writes two lines.
        /// </summary>
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

        /// <summary>
        /// print(1 + 2) -> "3".
        /// </summary>
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

        /// <summary>
        /// print (10 / 4.0) respects invariant culture -> "2.5".
        /// </summary>
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

        /// <summary>
        /// print((2 + 3) * 4) -> "20".
        /// </summary>
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
        
        /// <summary>
        /// Captures <see cref="Console.Out"/> while executing provided action and returns the trimmed output.
        /// </summary>
        private string CaptureConsole(Action action) // Utilitaire
        {
            var originalOut = Console.Out;
            var writer = new StringWriter();            
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

        /// <summary>
        /// bind x:int = 123; should not throw.
        /// </summary>
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

        /// <summary>
        /// set y:int = 10 +5; should not throw.
        /// </summary>
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

        /// <summary>
        /// (4 + 5) should evaluate to 9.
        /// </summary>
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

        /// <summary>
        /// print(2 + 3) -> "5".
        /// </summary>
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

        /// <summary>
        /// print(42) -> "42".
        /// </summary>
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

        /// <summary>
        /// print(3.14) with invariant culture -> "3.14".
        /// </summary>
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

        /// <summary>
        /// print((1 + 2) * 2) -> "6".
        /// </summary>
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

        /// <summary>
        /// Assignment to a declared variable returns the new value.
        /// </summary>
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

        /// <summary>
        /// Assigning to an undeclared variable should throw.
        /// </summary>
        [Fact]
        public void Assignment_ToUndeclaredVariable_Throws()
        {
            var statements = new List<Statement>
            {
                new Assignment("x", new IntegerLiteral(5))
            };

            Assert.Throws<InvalidOperationException>(() => ILGeneratorRunner.GenerateAndRunIL(statements));
        }

        /// <summary>
        /// Reassigning to a constant should throw.
        /// </summary>
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
        #endregion

        ////////////////// 

        #region If-Else-Then
        /// <summary>
        /// If(true) assigns then-branch.
        /// </summary>
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

        /// <summary>
        /// If(false) assigns else-branch.
        /// </summary>
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

        /// <summary>
        /// IF(false) without else leaves value unchanged.
        /// </summary>
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

        /// <summary>
        /// If(true) without else excutes then-branch.
        /// </summary>
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

        /// <summary>
        /// Binary condition drives correct branch.
        /// </summary>
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

        /// <summary>
        /// Non-boolean if-condition should throw.
        /// </summary>
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

        /// <summary>
        /// Assigning to undeclared variable inside If should throw.
        /// </summary>
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
        #endregion

        #region While, Break, Continue
        /// <summary>
        /// While loop accumulates expected sum.
        /// </summary>
        [Fact]
        public void WhileStatement_LoopExecutesCorreclty()
        {
            var statements = new List<Statement>
            {
                new VariableDeclaration("x", new IntegerLiteral(0), new TypeAnnotation("int", TokenType.TypeInt)),
                new VariableDeclaration("i", new IntegerLiteral(0), new TypeAnnotation("int",TokenType.TypeInt)),
                new WhileStatement
                (
                    new BinaryExpression(new VariableReference("i"), OperatorType.Less, new IntegerLiteral(5)),
                    new Block(new List<Statement>
                    {
                        new Assignment("x", new BinaryExpression(new VariableReference("x"), OperatorType.Plus, new VariableReference("i"))),
                        new Assignment("i", new BinaryExpression(new VariableReference("i"), OperatorType.Plus, new IntegerLiteral(1)))
                    })
                ),
                new ExpressionStatement(new VariableReference("x"))
            };
            var result = ILGeneratorRunner.GenerateAndRunIL(statements);

            Assert.Equal(0 + 1 + 2 + 3 + 4, result);
        }

        /// <summary>
        /// Breaks exits the loop early.
        /// </summary>
        [Fact]
        public void WhileStatement_WithBreak_StopsLoopEarly()
        {
            var statements = new List<Statement>
            {
                new VariableDeclaration("x", new IntegerLiteral(0), new TypeAnnotation("int", TokenType.TypeInt)),
                new VariableDeclaration("i", new IntegerLiteral(0), new TypeAnnotation("int", TokenType.TypeInt)),
                new WhileStatement(
                    new BooleanLiteral(true),
                    new Block(new List<Statement>
                    {
                        new IfStatement(
                            new BinaryExpression(new VariableReference("i"), OperatorType.Equalequal, new IntegerLiteral(3)),
                            new BreakStatement()
                        ),
                        new Assignment("x", new BinaryExpression(new VariableReference("x"), OperatorType.Plus, new VariableReference("i"))),
                        new Assignment("i", new BinaryExpression(new VariableReference("i"), OperatorType.Plus, new IntegerLiteral(1)))
                    })
                ),
                new ExpressionStatement(new VariableReference("x"))
            };
            var result = ILGeneratorRunner.GenerateAndRunIL(statements);

            Assert.Equal(0 + 1 + 2, result);
        }

        /// <summary>
        /// Continue skips the iteration where i == 2.
        /// </summary>
        [Fact]
        public void WhileStatement_WithContinue_SkipsIteration()
        {
            var statements = new List<Statement>
            {
                new VariableDeclaration("x", new IntegerLiteral(0), new TypeAnnotation("int", TokenType.TypeInt)),
                new VariableDeclaration("i", new IntegerLiteral(0), new TypeAnnotation("int", TokenType.TypeInt)),
                new WhileStatement
                (
                    new BinaryExpression(new VariableReference("i"), OperatorType.Less, new IntegerLiteral(5)),
                    new Block(new List<Statement>
                    {
                        new IfStatement
                        (
                            new BinaryExpression(
                                new BinaryExpression(new VariableReference("i"), OperatorType.Equalequal, new IntegerLiteral(2)),
                                OperatorType.AndAnd,
                                new BooleanLiteral(true)
                            ),
                            new Block(new List<Statement>
                            {
                                new Assignment("i", new BinaryExpression(new VariableReference("i"), OperatorType.Plus, new IntegerLiteral(1))),
                                new ContinueStatement()
                            })
                        ),
                        new Assignment("x", new BinaryExpression(new VariableReference("x"), OperatorType.Plus, new VariableReference("i"))),
                        new Assignment("i", new BinaryExpression(new VariableReference("i"), OperatorType.Plus, new IntegerLiteral(1)))
                    })
                ),
                new ExpressionStatement(new VariableReference("x"))
            };
            var result = ILGeneratorRunner.GenerateAndRunIL(statements);

            Assert.Equal(1 + 3 + 4, result); 
        }

        /// <summary>
        /// Break outside of loop should throw.
        /// </summary>
        [Fact]
        public void BreakStatement_OutsideLoop_ThrowsException()
        {
            var statements = new List<Statement>
            {
                new BreakStatement()
            };

            var error = Assert.Throws<InvalidOperationException>(() =>
            {
                ILGeneratorRunner.GenerateAndRunIL(statements);
            });
            Assert.Contains("break", error.Message, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Continue outside of loop should throw.
        /// </summary>
        [Fact]
        public void ContinueStatement_OutsideLoop_ThrowsException()
        {
            var statements = new List<Statement>
            {
                new ContinueStatement()
            };

            var error = Assert.Throws<InvalidOperationException>(() =>
            {
                ILGeneratorRunner.GenerateAndRunIL(statements);
            });
            Assert.Contains("continue", error.Message, StringComparison.OrdinalIgnoreCase);
        }
        #endregion

        #region forstatement
        /// <summary>
        /// Basic for-loop sums 0..4 = 10.
        /// </summary>
        [Fact]
        public void ForStatement_BasicLoopWorks()
        {
            var statements = new List<Statement>
            {
                new VariableDeclaration("sum", new IntegerLiteral(0), new TypeAnnotation("int", TokenType.TypeInt)),
                new ForStatement
                (
                    new VariableDeclaration("i", new IntegerLiteral(0), new TypeAnnotation("int", TokenType.TypeInt)),
                    new BinaryExpression(new VariableReference("i"), OperatorType.Less, new IntegerLiteral(5)),
                    new Assignment("i", new BinaryExpression(new VariableReference("i"), OperatorType.Plus, new IntegerLiteral(1))),
                    new Block(new List<Statement>
                    {
                        new Assignment("sum", new BinaryExpression(new VariableReference("sum"), OperatorType.Plus, new VariableReference("i")))
                    })
                ),
                new ExpressionStatement(new VariableReference("sum"))
            };
            var result = ILGeneratorRunner.GenerateAndRunIL(statements);

            Assert.Equal(10, result); // 0 + 1 + 2 + 3 + 4 = 10
        }

        /// <summary>
        /// for-loop with break stops at i == 3.
        /// </summary>
        [Fact]
        public void ForStatement_WithBreakStopsEarly()
        {
            var statements = new List<Statement>
            {
                new VariableDeclaration("sum", new IntegerLiteral(0), new TypeAnnotation("int", TokenType.TypeInt)),
                new ForStatement
                (
                    new VariableDeclaration("i", new IntegerLiteral(0), new TypeAnnotation("int", TokenType.TypeInt)),
                    new BinaryExpression(new VariableReference("i"), OperatorType.Less, new IntegerLiteral(5)),
                    new Assignment("i", new BinaryExpression(new VariableReference("i"), OperatorType.Plus, new IntegerLiteral(1))),
                    new Block(new List<Statement>
                    {
                        new IfStatement(
                            new BinaryExpression(new VariableReference("i"), OperatorType.Equalequal, new IntegerLiteral(3)),
                            new BreakStatement()
                        ),
                        new Assignment("sum", new BinaryExpression(new VariableReference("sum"), OperatorType.Plus, new VariableReference("i")))
                    })
                ),
                new ExpressionStatement(new VariableReference("sum"))
            };
            var result = ILGeneratorRunner.GenerateAndRunIL(statements);

            Assert.Equal(3, result); // 0 + 1 + 2 = 3
        }

        /// <summary>
        /// for-loop with continue skips i == 2; sum = 8.
        /// </summary>
        [Fact]
        public void ForStatement_WithContinue_SkipsValue()
        {
            var statements = new List<Statement>
            {
                new VariableDeclaration("sum", new IntegerLiteral(0), new TypeAnnotation("int", TokenType.TypeInt)),
                new ForStatement
                (
                    new VariableDeclaration("i", new IntegerLiteral(0), new TypeAnnotation("int", TokenType.TypeInt)),
                    new BinaryExpression(new VariableReference("i"), OperatorType.Less, new IntegerLiteral(5)),
                    new Assignment("i", new BinaryExpression(new VariableReference("i"), OperatorType.Plus, new IntegerLiteral(1))),
                    new Block(new List<Statement>
                    {
                        new IfStatement(
                            new BinaryExpression(new VariableReference("i"), OperatorType.Equalequal, new IntegerLiteral(2)),
                            new ContinueStatement()
                        ),
                        new Assignment("sum", new BinaryExpression(new VariableReference("sum"), OperatorType.Plus, new VariableReference("i")))
                    })
                ),
                new ExpressionStatement(new VariableReference("sum"))
            };
            var result = ILGeneratorRunner.GenerateAndRunIL(statements);

            Assert.Equal(8, result); // 0 + 1 + 3 + 4 = 8
        }

        /// <summary>
        /// for-loop with zero iterations leaves value unchanged.
        /// </summary>
        [Fact]
        public void ForStatement_WithZeroIterations_DoesNothing()
        {
            var statements = new List<Statement>
            {
                new VariableDeclaration("sum", new IntegerLiteral(42), new TypeAnnotation("int", TokenType.TypeInt)),
                new ForStatement
                (
                    new VariableDeclaration("i", new IntegerLiteral(10), new TypeAnnotation("int", TokenType.TypeInt)),
                    new BinaryExpression(new VariableReference("i"), OperatorType.Less, new IntegerLiteral(5)),
                    new Assignment("i", new BinaryExpression(new VariableReference("i"), OperatorType.Plus, new IntegerLiteral(1))),
                    new Block(new List<Statement>
                    {
                        new Assignment("sum", new IntegerLiteral(0)) // should never execute
                    })
                ),
                new ExpressionStatement(new VariableReference("sum"))
            };
            var result = ILGeneratorRunner.GenerateAndRunIL(statements);

            Assert.Equal(42, result); // unchanged
        }

        /// <summary>
        /// Non-boolean for-condition should throw.
        /// </summary>
        [Fact]
        public void ForStatement_WithNonBooleanCondition_Throws()
        {
            var statements = new List<Statement>
            {
                new VariableDeclaration("sum", new IntegerLiteral(0), new TypeAnnotation("int", TokenType.TypeInt)),

                new ForStatement
                (
                    new VariableDeclaration("i", new IntegerLiteral(0), new TypeAnnotation("int", TokenType.TypeInt)),
                    new BinaryExpression(
                        new VariableReference("i"),
                        OperatorType.Plus,
                        new IntegerLiteral(1)
                    ),
                    new Assignment("i", new BinaryExpression(
                        new VariableReference("i"),
                        OperatorType.Plus,
                        new IntegerLiteral(1))),
                    new Block(new List<Statement>
                    {
                        new Assignment("sum", new BinaryExpression(
                            new VariableReference("sum"),
                            OperatorType.Plus,
                            new VariableReference("i")))
                    })
                )
            };

            var error = Assert.Throws<InvalidOperationException>(() =>
            {
                ILGeneratorRunner.GenerateAndRunIL(statements);
            });
            Assert.Contains("The condition in for-statement must be of type 'bool'", error.Message);
        }
        #endregion

        #region Input()
        /// <summary>
        /// input() into int-annotated variable parses int.
        /// </summary>
        [Fact]
        public void Input_ReadInt_ReturnsParsedInt()
        {
            var statements = new List<Statement>
            {
                new VariableDeclaration("x", new FunctionCall("input", new List<Expression>()), new TypeAnnotation("int", TokenType.TypeInt)),
                new ExpressionStatement(new VariableReference("x"))
            };
            var result = ILGeneratorRunner.GenerateAndRunIL(statements, input: "42");

            Assert.Equal(42, result);
        }

        /// <summary>
        /// input() into bool-annotated variable parses bool.
        /// </summary>
        [Fact]
        public void Input_ReadBool_ReturnsParsedBool()
        {
            var statements = new List<Statement>
            {
                new VariableDeclaration("flag", new FunctionCall("input", new List<Expression>()), new TypeAnnotation("bool", TokenType.TypeBool)),
                new ExpressionStatement(new VariableReference("flag"))
            };
            var result = ILGeneratorRunner.GenerateAndRunIL(statements, input: "true");

            Assert.Equal(true, result);
        }

        /// <summary>
        /// input() into string-annotated variable returns string.
        /// </summary>
        [Fact]
        public void Input_ReadString_ReturnsString()
        {
            var statements = new List<Statement>
            {
                new VariableDeclaration("s", new FunctionCall("input", new List<Expression>()), new TypeAnnotation("string", TokenType.TypeString)),
                new ExpressionStatement(new VariableReference("s"))
            };
            var result = ILGeneratorRunner.GenerateAndRunIL(statements, input: "hello world");

            Assert.Equal("hello world", result);
        }

        /// <summary>
        /// input() with argument should throw.
        /// </summary>
        [Fact]
        public void Input_WithArgument_Throws()
        {
            var statement = new List<Statement>
            {
                new VariableDeclaration("x", new FunctionCall("input", new List<Expression> { new IntegerLiteral(5) }), new TypeAnnotation("int", TokenType.TypeInt)),
                new ExpressionStatement(new VariableReference("x"))
            };

            Assert.Throws<InvalidOperationException>(() => ILGeneratorRunner.GenerateAndRunIL(statement, input: "5")); 
        }

        /// <summary>
        /// input() into unsupported type should throw.
        /// </summary>
        [Fact]
        public void Input_WithUnsupportedType_Throws()
        {
            var statements = new List<Statement>
            {
                new VariableDeclaration("t", new FunctionCall("input", new List<Expression>()), new TypeAnnotation("tinyint", TokenType.Identifier)), // Incorrect volontairement
                new ExpressionStatement(new VariableReference("t"))
            };

            Assert.Throws<NotSupportedException>(() => ILGeneratorRunner.GenerateAndRunIL(statements, input: "a"));
        }
        #endregion

        #region FunctionCall
        /// <summary>
        /// User-defined function without args that prints "Hello!".
        /// </summary>
        [Fact]
        public void FunctionCall_NoArguments_Works()
        {
            var statements = new List<Statement>
                {
                    new FunctionDeclaration(
                        "hello",
                        new List<string>(),
                        new Block(new List<Statement>
                        {
                            new ExpressionStatement(
                                new FunctionCall("print", new List<Expression>
                                {
                                    new StringLiteral("Hello!")
                                })
                            )
                        })
                    ),
					// Call : hello()
					new ExpressionStatement(new FunctionCall("hello", new List<Expression>()))
                };
            var output = CaptureConsole(() =>
            {
                ILGeneratorRunner.GenerateAndRunIL(statements);
            });

            Assert.Equal("Hello!", output);
        }

        /// <summary>
        /// User-defined function with one argument (echo via print).
        /// </summary>
        [Fact]
        public void FunctionCall_WithOneArgument_Works()
        {
            var statements = new List<Statement>
                {
                    new FunctionDeclaration(
                        "greet",
                        new List<string> { "name" },
                        new Block(new List<Statement>
                        {
							// To stay compatible without string concat, directly print the parameter
							new ExpressionStatement(
                                new FunctionCall("print", new List<Expression>
                                {
                                    new VariableReference("name")
                                })
                            )
                        })
                    ),
                    new ExpressionStatement(new FunctionCall("greet", new List<Expression>
                    {
                        new StringLiteral("Jordan")
                    }))
                };
            var output = CaptureConsole(() =>
            {
                ILGeneratorRunner.GenerateAndRunIL(statements);
            });

            Assert.Equal("Jordan", output);
        }

        /// <summary>
        /// User-defined function with two integer args (sum via print).
        /// </summary>
        [Fact]
        public void FunctionCall_WithMultipleArguments_Works()
        {
            var statements = new List<Statement>
                {
                    new FunctionDeclaration(
                        "sum",
                        new List<string> { "a", "b" },
                        new Block(new List<Statement>
                        {
							// Integer addition, not string concat
							new ExpressionStatement(
                                new FunctionCall("print", new List<Expression>
                                {
                                    new BinaryExpression(new VariableReference("a"), OperatorType.Plus, new VariableReference("b"))
                                })
                            )
                        })
                    ),
                    new ExpressionStatement(new FunctionCall("sum", new List<Expression>
                    {
                        new IntegerLiteral(5),
                        new IntegerLiteral(7)
                    }))
                };
            var output = CaptureConsole(() =>
            {
                ILGeneratorRunner.GenerateAndRunIL(statements);
            });

            Assert.Equal("12", output);
        }

        /// <summary>
        /// Nested function call inside expression (five() + 3) printed as "8".
        /// </summary>
        [Fact]
        public void FunctionCall_NestedInsideExpression_Works()
        {
            var statements = new List<Statement>
                {
                    new FunctionDeclaration(
                        "five",
                        new List<string>(),
                        new Block(new List<Statement>
                        {
                            new ReturnStatement(new IntegerLiteral(5))
                        })
                    ),
                    new ExpressionStatement(
                        new FunctionCall("print", new List<Expression>
                        {
                            new BinaryExpression( new FunctionCall("five", new List<Expression>()), OperatorType.Plus, new IntegerLiteral(3))
                        })
                    )
                };
            var output = CaptureConsole(() =>
            {
                ILGeneratorRunner.GenerateAndRunIL(statements);
            });

            Assert.Equal("8", output);
        }
        #endregion

        #region concat string test
        /// <summary>
        /// "Hello " + "world" printed as "Hello world".
        /// </summary>
        [Fact]
        public void StringConcatenation_WithPlus_Works()
        {
            var statements = new List<Statement>
            {
                new ExpressionStatement(
                    new FunctionCall("print", new List<Expression>
                    {
                        new BinaryExpression(new StringLiteral("Hello "), OperatorType.Plus, new StringLiteral("world"))
                    })
                )
            };
            var output = CaptureConsole(() =>
            {
                ILGeneratorRunner.GenerateAndRunIL(statements);
            });

            Assert.Equal("Hello world", output);
        }
        #endregion
    }
}
