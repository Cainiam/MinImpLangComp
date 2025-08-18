using MinImpLangComp.AST;
using MinImpLangComp.Interpreting;
using Xunit;

namespace MinImpLangComp.Tests
{
    /// <summary>
    /// Control-flow tests for the tree-walking <see cref="Interpreter"/>. These tests assert that if/while/for constructs update the interpreter's environment as expected.
    /// </summary>
    public class InterpreterControlFlowTests
    {
        /// <summary>
        /// If the condition evaluates to "true" (non-zero), the then-branch is executed.
        /// After execution, the variabke <c>x</c> should be defined and set to 42.
        /// </summary>
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

        /// <summary>
        /// IF the condition evaluates to "false" (zero), the else-branch is executed.
        /// After execution, the variable <c>y</c> should be defined and set to 24.
        /// </summary>
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

        /// <summary>
        /// A while-loop should repeatedly execute its body while the condition is true.
        /// Here, x is incremented until it reaches 3.
        /// </summary>
        [Fact]
        public void Evaluate_WhileStatement_ExecutesBodyWhileConditionTrue()
        {
            var interp = new Interpreter();
            // Initialize x := 0 (implicit declaration via assignment in the interpreter).
            interp.Evaluate(new Assignment("x", new IntegerLiteral(0)));
            var whileStatement = new WhileStatement(
                new BinaryExpression(
                    new VariableReference("x"),
                    OperatorType.Less,
                    new IntegerLiteral(3)
                ),
                new Assignment(
                    "x",
                    new BinaryExpression(
                        new VariableReference("x"),
                        OperatorType.Plus,
                        new IntegerLiteral(1)
                    )
                )
            );
            interp.Evaluate(whileStatement);

            Assert.Equal(3, interp.GetEnvironment()["x"]);
        }

        /// <summary>
        /// A for-loop should initializer, then iterate while the condition is true, applying the increment after each body execution.
        /// At the end, x should hold the last value assigned in the loop, and 'i' should be at the first non-satisfying value.
        /// </summary>
        [Fact]
        public void Evaluate_ForStatement_ExecutesBodyForEachIteration()
        {
            var interp = new Interpreter();
            var initializer = new Assignment("i", new IntegerLiteral(1));
            var condition = new BinaryExpression(
                new VariableReference("i"), 
                OperatorType.Less, 
                new IntegerLiteral(3)
            );
            var increment = new Assignment(
                "i",
                new BinaryExpression(
                    new VariableReference("i"),
                    OperatorType.Plus,
                    new IntegerLiteral(1)
                )
            );
            var body = new Assignment("x", new VariableReference("i"));
            var forStatement = new ForStatement(initializer, condition, increment, body);
            interp.Evaluate(forStatement);

            // Last body execution assigns x = 2; then i is incremented to 3 and loop exits.
            Assert.Equal(2, interp.GetEnvironment()["x"]);
            Assert.Equal(3, interp.GetEnvironment()["i"]);
        }
    }
}
