using MinImpLangComp.AST;
using MinImpLangComp.Interpreting;
using Xunit;

namespace MinImpLangComp.Tests
{
    public class InterpreterTests
    {
        [Fact]
        public void Evaluate_IntegerLiteral_ReturnsIntegerValue()
        {
            var interp = new Interpreter();
            var node = new IntegerLiteral(1);
            var result = interp.Evaluate(node);

            Assert.IsType<int>(result);
            Assert.Equal(1, result);
        }

        [Fact]
        public void Evaluate_FloatLiteral_ReturnsFloatValue()
        {
            var interp = new Interpreter();
            var node = new FloatLiteral(1.23);
            var result = interp.Evaluate(node);

            Assert.IsType<double>(result);
            Assert.Equal(1.23, (double)result, 2);
        }

        [Fact]
        public void Evaluate_BinaryExpression_Addition_ReturnsSum()
        {
            var interp = new Interpreter();
            var node = new BinaryExpression(
                new IntegerLiteral(1),
                OperatorType.Plus,
                new IntegerLiteral(2)
            );
            var result = interp.Evaluate(node);

            Assert.IsType<int>(result);
            Assert.Equal(3, result);
        }

        [Fact]
        public void Evaluate_BinaryExpression_Multiplication_ReturnsProduct()
        {
            var interp = new Interpreter();
            var node = new BinaryExpression(
                new IntegerLiteral(5),
                OperatorType.Multiply,
                new IntegerLiteral(6)
            );
            var result = interp.Evaluate(node);

            Assert.IsType<int>(result);
            Assert.Equal(30, result);
        }

        [Fact]
        public void Evaluate_BinaryExpression_IntAndFloat_Addition_ReturnsSum()
        {
            var interp = new Interpreter();
            var node = new BinaryExpression(
                new FloatLiteral(5.5),
                OperatorType.Plus,
                new IntegerLiteral(4)
            );
            var result = interp.Evaluate(node);

            Assert.IsType<double>(result);
            Assert.Equal(9.5, (double)result, 2);
        }

        [Fact]
        public void Evaluate_Assignment_AddsVariableToEnvironment()
        {
            var interp = new Interpreter();
            var node = new Assignment("x",
                new BinaryExpression(
                    new IntegerLiteral(2),
                    OperatorType.Plus,
                    new IntegerLiteral(3)
                )
            );
            interp.Evaluate(node);
            var result = interp.GetEnvironment()["x"];

            Assert.IsType<int>(result);
            Assert.Equal(5, result);
        }

        [Fact]
        public void Evaluate_VariableReference_ReturnsStoredValue()
        {
            var interp = new Interpreter();
            interp.Evaluate(new Assignment("x", new IntegerLiteral(4)));
            var node = new VariableReference("x");
            var result = interp.Evaluate(node);

            Assert.IsType<int>(result);
            Assert.Equal(4, result);
        }

        [Fact]
        public void Evaluate_BlockWithMultipleStatements_ExecutesAll()
        {
            var interp = new Interpreter();
            var block = new Block(new List<Statement>
            {
                new Assignment("x", new IntegerLiteral(2)),
                new Assignment("y", new BinaryExpression(new VariableReference("x"), OperatorType.Plus, new IntegerLiteral(3))),
                new ExpressionStatement(new BinaryExpression(new VariableReference("y"), OperatorType.Multiply, new IntegerLiteral(2)))
            });
            var result = interp.Evaluate(block);

            Assert.IsType<int>(result);
            Assert.Equal(10, result);
            Assert.IsType<int>(interp.GetEnvironment()["x"]);
            Assert.Equal(2, interp.GetEnvironment()["x"]);
            Assert.IsType<int>(interp.GetEnvironment()["y"]);
            Assert.Equal(5, interp.GetEnvironment()["y"]);
        }

        [Fact]
        public void Evaluate_BinaryExpression_EqualEqual_ReturnsTrue()
        {
            var interp = new Interpreter();
            var node = new BinaryExpression(
                new IntegerLiteral(5),
                OperatorType.Equalequal,
                new IntegerLiteral(5)
            );
            var result = interp.Evaluate(node);

            Assert.IsType<bool>(result);
            Assert.True((bool)result);
        }

        [Fact]
        public void Evaluate_BinaryExpression_NotEqual_ReturnsTrue()
        {
            var interp = new Interpreter();
            var node = new BinaryExpression(
                new IntegerLiteral(3),
                OperatorType.NotEqual,
                new IntegerLiteral(4)
            );
            var result = interp.Evaluate(node);

            Assert.IsType<bool>(result);
            Assert.True((bool)result);
        }

        [Fact]
        public void Evaluate_BooleanLiteral_ReturnsBooleanValue()
        {
            var interp = new Interpreter();
            var trueNode = new BooleanLiteral(true);
            var falseNode = new BooleanLiteral(false);
            var trueResult = interp.Evaluate(trueNode);
            var falseResult = interp.Evaluate(falseNode);

            Assert.IsType<bool>(trueResult);
            Assert.True((bool)trueResult);
            Assert.IsType<bool>(falseResult);
            Assert.False((bool)falseResult);
        }

        [Fact]
        public void Evaluate_IfStatement_WithBooleanLiteralCondition_WorksCorreclty()
        {
            var interp = new Interpreter();
            var ifStatement = new IfStatement(
                new BooleanLiteral(true),
                new Assignment("x", new IntegerLiteral(11)),
                new Assignment("x", new IntegerLiteral(22))
            );
            interp.Evaluate(ifStatement);

            Assert.Equal(11, interp.GetEnvironment()["x"]);
        }

        [Fact]
        public void Evaluate_BinaryExpression_BooleanComparison_ReturnsCorrectResult()
        {
            var interp = new Interpreter();
            var node = new BinaryExpression(
                new IntegerLiteral(5),
                OperatorType.Less,
                new IntegerLiteral(10)
            );
            var result = interp.Evaluate(node);

            Assert.IsType<bool>(result);
            Assert.True((bool)result);
        }

        [Fact]
        public void Evaluate_LogicalAnd_ReturnsExpected()
        {
            var interp = new Interpreter();
            var node = new BinaryExpression(
                new BooleanLiteral(true),
                OperatorType.AndAnd,
                new BooleanLiteral(false)
            );
            var result = interp.Evaluate(node);

            Assert.IsType<bool>(result);
            Assert.False((bool)result);
        }

        [Fact]
        public void Evaluate_LogicalOr_ReturnsExpected()
        {
            var interp = new Interpreter();
            var node = new BinaryExpression(
                new BooleanLiteral(false),
                OperatorType.OrOr,
                new BooleanLiteral(true)
            );
            var result = interp.Evaluate(node);

            Assert.IsType<bool>(result);
            Assert.True((bool)result);
        }

        [Fact]
        public void Evaluate_UnaryIncrement_IncrementsVariable()
        {
            var interp = new Interpreter();
            interp.Evaluate(new Assignment("x", new IntegerLiteral(5)));
            var unary = new UnaryExpression(OperatorType.PlusPlus, "x");
            var result = interp.Evaluate(unary);

            Assert.Equal(6, result);
            Assert.Equal(6, interp.GetEnvironment()["x"]);
        }

        [Fact]
        public void Evaluate_UnaryDecrement_DecrementsVariable()
        {
            var interp = new Interpreter();
            interp.Evaluate(new Assignment("y", new IntegerLiteral(3)));
            var unary = new UnaryExpression(OperatorType.MinusMinus, "y");
            var result = interp.Evaluate(unary);

            Assert.Equal(2, result);
            Assert.Equal(2, interp.GetEnvironment()["y"]);
        }

        [Fact]
        public void Evaluate_PrintStatement_PrintsValue()
        {
            var interp = new Interpreter();
            interp.Evaluate(new Assignment("x", new IntegerLiteral(42)));
            var printStatement = new PrintStatement(new VariableReference("x"));
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                var result = interp.Evaluate(printStatement);
                Assert.Equal(42, result);
                var outpout = sw.ToString().Trim();
                Assert.Equal("42", outpout);
            }
        }
    }
}