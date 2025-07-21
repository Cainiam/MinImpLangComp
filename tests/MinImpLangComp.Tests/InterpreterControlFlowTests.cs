using MinImpLangComp.AST;
using MinImpLangComp.Interpreting;

namespace MinImpLangComp.Tests
{
    public class InterpreterControlFlowTests
    {
        [Fact]
        public void Evaluate_IfStatement_ExecutesThenBranch_WhenConditionIsTrue()
        {
            var interp = new Interpreter();
            var ifStatement = new IfStatement(
                new IntegerLiteral(1),
                new Assignment("x", new IntegerLiteral(42)),
                null
            );
            interp.Evaluate(ifStatement);

            Assert.True(interp.GetEnvironment().ContainsKey("x"));
            Assert.Equal(42, interp.GetEnvironment()["x"]);
        }

        [Fact]
        public void Evaluate_IfStatement_ExecutesElseBranch_WhenConditionIsFalse()
        {
            var interp = new Interpreter();
            var ifStatement = new IfStatement(
                new IntegerLiteral(0),
                null,
                new Assignment("y", new IntegerLiteral(24))
            );
            interp.Evaluate(ifStatement);

            Assert.True(interp.GetEnvironment().ContainsKey("y"));
            Assert.Equal(24, interp.GetEnvironment()["y"]);
        }

        [Fact]
        public void Evaluate_WhileStatement_ExecutesBodyWhileConditionTrue()
        {
            var interp = new Interpreter();
            interp.Evaluate(new Assignment("x", new IntegerLiteral(0)));
            var whileStatement = new WhileStatement(
                new BinaryExpression(
                    new VariableReference("x"),
                    "<",
                    new IntegerLiteral(3)
                ),
                new Assignment(
                    "x",
                    new BinaryExpression(
                        new VariableReference("x"),
                        "+",
                        new IntegerLiteral(1)
                    )
                )
            );
            interp.Evaluate(whileStatement);

            Assert.Equal(3, interp.GetEnvironment()["x"]);
        }

        [Fact]
        public void Evaluate_ForStatement_ExecutesBodyForEachIteration()
        {
            var interp = new Interpreter();
            var initializer = new Assignment("i", new IntegerLiteral(1));
            var condition = new BinaryExpression(
                new VariableReference("i"), 
                "<", 
                new IntegerLiteral(3)
            );
            var increment = new Assignment(
                "i",
                new BinaryExpression(
                    new VariableReference("i"),
                    "+",
                    new IntegerLiteral(1)
                )
            );
            var body = new Assignment("x", new VariableReference("i"));
            var forStatement = new ForStatement(initializer, condition, increment, body);
            interp.Evaluate(forStatement);

            Assert.Equal(2, interp.GetEnvironment()["x"]);
            Assert.Equal(3, interp.GetEnvironment()["i"]);
        }
    }
}
