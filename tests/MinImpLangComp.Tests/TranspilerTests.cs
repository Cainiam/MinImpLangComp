using MinImpLangComp.Transpiling;
using MinImpLangComp.AST;
using Xunit;

namespace MinImpLangComp.Tests
{
    public class TranspilerTests
    {
        [Fact]
        public void Transpile_ReturnsValideCSharpSkeleton()
        {
            var transpiler = new Transpiler();
            var block = new Block(new List<Statement>());
            var result = transpiler.Transpile(block);

            Assert.Contains("namespace MinImpLangComp", result);
            Assert.Contains("static void Main", result);
        }

        [Fact]
        public void Transpile_PrintGeneratesConsoleWriteLine()
        {
            var program = new Block(new List<Statement>
            {
                new ExpressionStatement(new FunctionCall("print", new List<Expression>
                {
                    new IntegerLiteral(5)
                }))
            });
            var transpiler = new Transpiler();
            var result = transpiler.Transpile(program);

            //Console.WriteLine(result);

            Assert.Contains("Console.WriteLine(5);", result);
            Assert.Matches(@"Console\.WriteLine\s*\(\s*5\s*\);", result);
        }

        [Fact]
        public void Transpile_AssignmentGeneratesVariableDeclaration()
        {
            var program = new Block(new List<Statement>
            {
                new Assignment("x", new IntegerLiteral(10))
            });
            var transpiler = new Transpiler();
            var result = transpiler.Transpile(program);

            //Console.WriteLine(result);

            Assert.Contains("var x = 10;", result);
        }

        [Fact]
        public void Transpile_ArtihmeticExpressionGeneratesCorrectCSharp()
        {
            var expression = new BinaryExpression(
                new IntegerLiteral(5),
                OperatorType.Plus,
                new BinaryExpression(
                    new IntegerLiteral(3),
                    OperatorType.Multiply,
                    new IntegerLiteral(2)
                )
            );
            var program = new Block(new List<Statement>
            {
                new Assignment("x", expression)
            });
            var transpiler = new Transpiler();
            var result = transpiler.Transpile(program);

            //Console.WriteLine(result);

            Assert.Contains("var x = (5 + (3 * 2));", result);
        }

        [Fact]
        public void Transpile_IfStatementGeneratesCorrectCSharp()
        {
            var ifStmt = new IfStatement(
                new BinaryExpression(
                    new VariableReference("x"),
                    OperatorType.Greater,
                    new IntegerLiteral(5)
                ),
                new ExpressionStatement(new FunctionCall("print", new List<Expression> { new VariableReference("x") }))
            );
            var program = new Block(new List<Statement> { ifStmt });
            var transpiler = new Transpiler();
            var result = transpiler.Transpile(program);

            //Console.WriteLine(result);

            Assert.Contains("if ((x > 5))", result);
            Assert.Contains("Console.WriteLine(x);", result);
        }

        [Fact]
        public void Transpile_WhileStatementGeneratesCorrectCsharp()
        {
            var whileStament = new WhileStatement(
                new BooleanLiteral(true),
                new ExpressionStatement(new FunctionCall("print", new List<Expression> { new StringLiteral("Loop") }))
            );
            var program = new Block(new List<Statement> { whileStament });
            var transpiler = new Transpiler();
            var result = transpiler.Transpile(program);

            Console.WriteLine(result);

            Assert.Contains("while (true)", result);
            Assert.Contains("Console.WriteLine(\"Loop\");", result);
        }
    }
}
