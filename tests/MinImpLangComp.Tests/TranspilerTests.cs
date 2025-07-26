using MinImpLangComp.Transpiling;
using MinImpLangComp.AST;
using Xunit;
using Xunit.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace MinImpLangComp.Tests
{
    public class TranspilerTests
    {

        private readonly ITestOutputHelper _output;

        public TranspilerTests(ITestOutputHelper output)
        {
            _output = output;
        }

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
        public void Transpile_Print_GeneratesConsoleWriteLine()
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
        public void Transpile_Assignment_GeneratesVariableDeclaration()
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
        public void Transpile_ArtihmeticExpression_GeneratesCorrectCSharp()
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
        public void Transpile_IfStatement_GeneratesCorrectCSharp()
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
        public void Transpile_WhileStatement_GeneratesCorrectCsharp()
        {
            var whileStament = new WhileStatement(
                new BooleanLiteral(true),
                new ExpressionStatement(new FunctionCall("print", new List<Expression> { new StringLiteral("Loop") }))
            );
            var program = new Block(new List<Statement> { whileStament });
            var transpiler = new Transpiler();
            var result = transpiler.Transpile(program);

            //Console.WriteLine(result);

            Assert.Contains("while (true)", result);
            Assert.Contains("Console.WriteLine(\"Loop\");", result);
        }

        [Fact]
        public void Transpile_ForStatement_GeneratesCorrectCSharp()
        {
            var forStatement = new ForStatement(
                new Assignment("i", new IntegerLiteral(0)),
                new BinaryExpression(new VariableReference("i"), OperatorType.Less, new IntegerLiteral(10)),
                new Assignment("i", new BinaryExpression(new VariableReference("i"), OperatorType.Plus, new IntegerLiteral(1))),
                new ExpressionStatement(new FunctionCall("print", new List<Expression> { new VariableReference("i") }))
            );
            var program = new Block(new List<Statement> { forStatement });
            var transpiler = new Transpiler();
            var result = transpiler.Transpile(program);

            //Console.WriteLine(result);

            Assert.Contains("for (var i = 0; (i < 10); i = (i + 1))", result);
            Assert.Contains("Console.WriteLine(i);", result);
        }

        [Fact]
        public void Transpile_BlockStatement_GeneratesCurlyBracesAndIndentedStatements()
        {
            var block = new Block(new List<Statement>
            {
                new ExpressionStatement(new FunctionCall("print", new List<Expression> { new IntegerLiteral(1) })),
                new ExpressionStatement(new FunctionCall("print", new List<Expression> { new IntegerLiteral(2) }))
            });
            var ifStatement = new IfStatement(new BooleanLiteral(true), block);
            var program = new Block(new List<Statement> { ifStatement });
            var transpiler = new Transpiler();
            var result = transpiler.Transpile(program);

            //Console.WriteLine(result);

            Assert.Contains("if (true)", result);
            Assert.Contains("{", result);
            Assert.Contains("Console.WriteLine(1);", result);
            Assert.Contains("Console.WriteLine(2);", result);
            Assert.Contains("}", result);
        }

         [Fact]
         public void Transpile_FunctionDeclaration_GeneratesStaticVoidMethod()
         {
            var function = new FunctionDeclaration(
                "greet",
                new List<string>(),
                new Block(new List<Statement>
                {
                    new ExpressionStatement(new FunctionCall("print", new List<Expression> { new StringLiteral("Hello") }))
                })
            );
            var program = new Block(new List<Statement> { function });
            var transpiler = new Transpiler();
            var result = transpiler.Transpile(program);

            //Console.WriteLine(result);

            Assert.Contains("static void greet()", result);
            Assert.Contains("Console.WriteLine(\"Hello\")", result);
         }

        [Fact]
        public void Transpile_FunctionCall_GeneratesCorrectInvocation()
        {
            var functionDeclaration = new FunctionDeclaration(
                "greet",
                new List<string>(),
                new Block(new List<Statement>
                {
                    new ExpressionStatement(new FunctionCall("print", new List<Expression> { new StringLiteral("Hello") }))
                })
            );
            var functionCall = new ExpressionStatement(
                new FunctionCall("greet", new List<Expression>())
            );
            var program = new Block(new List<Statement> 
            { 
                functionDeclaration, 
                functionCall 
            });
            var transpiler = new Transpiler();
            var result = transpiler.Transpile(program);

            //Console.WriteLine(result);

            Assert.Contains("static void greet()", result);
            Assert.Contains("Console.WriteLine(\"Hello\")", result);
            Assert.Contains("greet();", result);
        }

        [Fact]
        public void Transpile_FunctionDeclarationWithParamAndCall_GeneratesCorrectCsharp()
        {
            var greetFunction = new FunctionDeclaration(
                "greet",
                new List<string> { "name" },
                new Block(new List<Statement> { 
                    new ExpressionStatement(
                        new FunctionCall("print", new List<Expression> { new VariableReference("name") })
                    )
                })
            );
            var call = new ExpressionStatement(new FunctionCall("greet", new List<Expression> { new StringLiteral("Alice") }));
            var program = new Block(new List<Statement> { greetFunction, call });
            var transpiler = new Transpiler();
            var result = transpiler.Transpile(program);

            //Console.WriteLine(result);

            Assert.Contains("static void greet(dynamic name)", result);
            Assert.Contains("Console.WriteLine(name);", result);
            Assert.Contains("greet(\"Alice\");", result);
        }
    }
}
