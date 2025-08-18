using MinImpLangComp.Transpiling;
using MinImpLangComp.AST;
using MinImpLangComp.Lexing;
using Xunit;

namespace MinImpLangComp.Tests
{
    /// <summary>
    /// Unit tests for the <see cref="Transpiler"/> that verify the generated
    /// C# source matches expected patterns for a variety of AST shapes.
    /// </summary>
    public class TranspilerTests
    {
        #region Helper
        /// <summary>
        /// Small helper to keep tests concise.
        /// Builds a <see cref="Transpiler"/> and returns the transpiled C# string
        /// for the provided program block.
        /// </summary>
        private static string Transpile(Block program) => new Transpiler().Transpile(program);
        #endregion

        /// <summary>
        /// Ensures the transpiler returns a minimal but valid C# program skeleton.
        /// </summary>
        [Fact]
        public void Transpile_ReturnsValideCSharpSkeleton()
        {
            var result = Transpile(new Block(new List<Statement>()));

            Assert.Contains("namespace MinImpLangComp", result);
            Assert.Contains("static void Main", result);
        }

        /// <summary>
        /// Validates that a print call becomes a Console.WriteLine invocation.
        /// </summary>
        [Fact]
        public void Transpile_Print_GeneratesConsoleWriteLine()
        {
            var program = new Block(new List<Statement>
            {
                new ExpressionStatement(new FunctionCall("print", new List<Expression> { new IntegerLiteral(5) }))
            });
            var result = Transpile(program);

            Assert.Contains("Console.WriteLine(5);", result);
            Assert.Matches(@"Console\.WriteLine\s*\(\s*5\s*\);", result);
        }

        /// <summary>
        /// Assignments should become <c>var name = expr;</c>.
        /// </summary>
        [Fact]
        public void Transpile_Assignment_GeneratesVariableDeclaration()
        {
            var program = new Block(new List<Statement>
            {
                new Assignment("x", new IntegerLiteral(10))
            });
            var result = Transpile(program);

            Assert.Contains("var x = 10;", result);
        }

        /// <summary>
        /// Arithmetic expressions must be parenthesized to preserve precedence.
        /// </summary>
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
            var program = new Block(new List<Statement> { new Assignment("x", expression) });
            var result = Transpile(program);

            Assert.Contains("var x = (5 + (3 * 2));", result);
        }

        /// <summary>
        /// If-statements should produce an <c>if</c> header and its body.
        /// </summary>
        [Fact]
        public void Transpile_IfStatement_GeneratesCorrectCSharp()
        {
            var ifStmt = new IfStatement(
                new BinaryExpression(new VariableReference("x"), OperatorType.Greater, new IntegerLiteral(5)),
                new ExpressionStatement(new FunctionCall("print", new List<Expression> { new VariableReference("x") }))
            );
            var program = new Block(new List<Statement> { ifStmt });
            var result = Transpile(program);

            Assert.Contains("if ((x > 5))", result);
            Assert.Contains("Console.WriteLine(x);", result);
        }

        /// <summary>
        /// While-statements should produce the expected C# loop and body.
        /// </summary>
        [Fact]
        public void Transpile_WhileStatement_GeneratesCorrectCsharp()
        {
            var whileStament = new WhileStatement(
                new BooleanLiteral(true),
                new ExpressionStatement(new FunctionCall("print", new List<Expression> { new StringLiteral("Loop") }))
            );
            var program = new Block(new List<Statement> { whileStament });
            var result = Transpile(program);

            Assert.Contains("while (true)", result);
            Assert.Contains("Console.WriteLine(\"Loop\");", result);
        }

        /// <summary>
        /// For-statements should become standard C# for-loops.
        /// </summary>
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
            var result = Transpile(program);

            Assert.Contains("for (var i = 0; (i < 10); i = (i + 1))", result);
            Assert.Contains("Console.WriteLine(i);", result);
        }

        /// <summary>
        /// Blocks should be wrapped in curly braces with inner statements inside.
        /// </summary>
        [Fact]
        public void Transpile_BlockStatement_GeneratesCurlyBracesAndIndentedStatements()
        {
            var block = new Block(new List<Statement>
            {
                new ExpressionStatement(new FunctionCall("print", new List<Expression> { new IntegerLiteral(1) })),
                new ExpressionStatement(new FunctionCall("print", new List<Expression> { new IntegerLiteral(2) }))
            });
            var program = new Block(new List<Statement> { new IfStatement(new BooleanLiteral(true), block) });
            var result = Transpile(program);

            Assert.Contains("if (true)", result);
            Assert.Contains("{", result);
            Assert.Contains("Console.WriteLine(1);", result);
            Assert.Contains("Console.WriteLine(2);", result);
            Assert.Contains("}", result);
        }

        /// <summary>
        /// Function declarations without parameters should transpile to static void methods.
        /// </summary>
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
            var result = Transpile(program);

            Assert.Contains("static void greet()", result);
            Assert.Contains("Console.WriteLine(\"Hello\")", result);
        }

        /// <summary>
        /// Function calls should be emitted as method invocations.
        /// </summary>
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
            var functionCall = new ExpressionStatement(new FunctionCall("greet", new List<Expression>()));
            var program = new Block(new List<Statement> { functionDeclaration, functionCall });
            var result = Transpile(program);

            Assert.Contains("static void greet()", result);
            Assert.Contains("Console.WriteLine(\"Hello\")", result);
            Assert.Contains("greet();", result);
        }

        /// <summary>
        /// Functions with parameters should emit parameters and use them in the body.
        /// </summary>
        [Fact]
        public void Transpile_FunctionDeclarationWithParamAndCall_GeneratesCorrectCsharp()
        {
            var greetFunction = new FunctionDeclaration(
                "greet",
                new List<string> { "name" },
                new Block(new List<Statement>
                {
                    new ExpressionStatement(new FunctionCall("print", new List<Expression> { new VariableReference("name") }))
                })
            );
            var call = new ExpressionStatement(new FunctionCall("greet", new List<Expression> { new StringLiteral("Alice") }));
            var program = new Block(new List<Statement> { greetFunction, call });
            var result = Transpile(program);

            Assert.Contains("static void greet(dynamic name)", result);
            Assert.Contains("Console.WriteLine(name);", result);
            Assert.Contains("greet(\"Alice\");", result);
        }

        /// <summary>
        /// The built-in input() should map to Console.ReadLine().
        /// </summary>
        [Fact]
        public void Transpile_InputCall_GeneratesConsoleReadLine()
        {
            var program = new Block(new List<Statement>
            {
                new Assignment("x", new FunctionCall("input", new List<Expression>()))
            });
            var result = Transpile(program);

            Assert.Contains("var x = Console.ReadLine();", result);
        }

        /// <summary>
        /// ++ and -- unary expressions should be emitted as C# increment/decrement.
        /// </summary>
        [Fact]
        public void Transpile_UnaryINcrementDecrement_GeneratesCorrectCSharp()
        {
            var program = new Block(new List<Statement>
            {
                new ExpressionStatement(new UnaryExpression(OperatorType.PlusPlus, "x")),
                new ExpressionStatement(new UnaryExpression(OperatorType.MinusMinus, "y"))
            });
            var result = Transpile(program);

            Assert.Contains("x++;", result);
            Assert.Contains("y--;", result);
        }

        /// <summary>
        /// A function containing a return statement should emit a C# return.
        /// </summary>
        [Fact]
        public void Transpile_FunctionWithReturnStatement_ShouldGenerateReturn()
        {
            var func = new FunctionDeclaration(
                "add",
                new List<string> { "x", "y" },
                new Block(new List<Statement>
                {
                    new ReturnStatement(new BinaryExpression(new VariableReference("x"), OperatorType.Plus, new VariableReference("y")))
                })
            );
            var program = new Block(new List<Statement> { func });
            var result = Transpile(program);

            Assert.Contains("return (x + y);", result);
        }

        /// <summary>
        /// The null literal should be emitted as the C# keyword <c>null</c>.
        /// </summary>
        [Fact]
        public void Transpile_NullLiteral_ShouldGenerateNullKeyword()
        {
            var program = new Block(new List<Statement> { new ExpressionStatement(new NullLiteral()) });
            var result = Transpile(program);

            Assert.Contains("null;", result);
        }

        /// <summary>
        /// break/continue statements should appear in the generated C# code.
        /// </summary>
        [Fact]
        public void Transpile_BreakAndContinue_ShouldGenerateValidCSharp()
        {
            var program = new Block(new List<Statement>
            {
                new WhileStatement(
                    new BooleanLiteral(true),
                    new Block(new List<Statement> { new BreakStatement(), new ContinueStatement() })
                )
            });
            var result = Transpile(program);

            Assert.Contains("break;", result);
            Assert.Contains("continue;", result);
        }

        /// <summary>
        /// Variable declarations without type annotations should use <c>var</c>.
        /// </summary>
        [Fact]
        public void Transpiler_TranspilesVariableDeclarationToVar()
        {
            var declaration = new VariableDeclaration("x", new IntegerLiteral(42), null);
            var block = new Block(new List<Statement> { declaration });
            var result = Transpile(block);

            Assert.Contains("var x = 42;", result);
        }

        /// <summary>
        /// Constant declarations without type annotations should also use <c>var</c>.
        /// </summary>
        [Fact]
        public void Transpiler_TranspilesConstantDeclarationToVar()
        {
            var declaration = new ConstantDeclaration("y", new IntegerLiteral(90), null);
            var block = new Block(new List<Statement> { declaration });
            var result = Transpile(block);

            Assert.Contains("var y = 90;", result);
        }

        /// <summary>
        /// Variable declarations with an int annotation should emit <c>int</c>.
        /// </summary>
        [Fact]
        public void Transpiler_TranspilesVariableDeclarationWithTypeAnnotation_Int()
        {
            var declaration = new VariableDeclaration("x", new IntegerLiteral(42), new TypeAnnotation("int", TokenType.TypeInt));
            var block = new Block(new List<Statement> { declaration });
            var result = Transpile(block);

            Assert.Contains("int x = 42;", result);
        }

        /// <summary>
        /// Variable declarations with a bool annotation should emit <c>bool</c>.
        /// </summary>
        [Fact]
        public void Transpiler_TranspilesVariableDeclarationWithTypeAnnotation_Bool()
        {
            var declaration = new VariableDeclaration("flag", new BooleanLiteral(true), new TypeAnnotation("bool", TokenType.TypeBool));
            var block = new Block(new List<Statement> { declaration });
            var result = Transpile(block);

            Assert.Contains("bool flag = true;", result);
        }

        /// <summary>
        /// Constant declarations with a string annotation should emit <c>string</c>.
        /// </summary>
        [Fact]
        public void Transpiler_TranspilesConstantDeclarationWithTypeAnnotation_String()
        {
            var declaration = new ConstantDeclaration("msg", new StringLiteral("Hello"), new TypeAnnotation("string", TokenType.TypeString));
            var block = new Block(new List<Statement> { declaration });
            var result = Transpile(block);

            Assert.Contains("string msg = \"Hello\";", result);
        }
    }
}
