using MinImpLangComp.AST;
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
    }
}
