using MinImpLangComp.AST;
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
                "+",
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
                "*",
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
                "+",
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
                    "+",
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
                new Assignment("y", new BinaryExpression(new VariableReference("x"), "+", new IntegerLiteral(3))),
                new ExpressionStatement(new BinaryExpression(new VariableReference("y"), "*", new IntegerLiteral(2)))
            });
            var result = interp.Evaluate(block);

            Assert.IsType<int>(result);
            Assert.Equal(10, result);
            Assert.IsType<int>(interp.GetEnvironment()["x"]);
            Assert.Equal(2, interp.GetEnvironment()["x"]);
            Assert.IsType<int>(interp.GetEnvironment()["y"]);
            Assert.Equal(5, interp.GetEnvironment()["y"]);
        }

    }
}
