using MinImpLangComp.AST;
using MinImpLangComp.Exceptions;
using MinImpLangComp.Interpreting;
using MinImpLangComp.Lexing;
using Xunit;

namespace MinImpLangComp.Tests
{
    /// <summary>
    /// Unit tests for the tree-walking <see cref="Interpreter"/>. Marked with "ConsoleSerial" to avoid concurrency on Console I/O.
    /// </summary>
    [Collection("ConsoleSerial")]
    public class InterpreterTests
    {
        #region Helper
        /// <summary>
        /// Capture <see cref="Console.Out"/> during the provided action, then restore it. Returns the trimmed captured output.
        /// </summary>
        /// <param name="action"></param>
        private static string CaptureOut(Action action)
        {
            var originalOut = Console.Out;
            try
            {
                using var sw = new StringWriter();
                Console.SetOut(sw);
                action();
                return sw.ToString().Trim();
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        /// <summary>
        /// Temporarily sets <see cref="Console.In"/> from the provided string while executing <paramref name="func"/>, then restore the original input.
        /// Returns the function's result.
        /// </summary>
        private static T WithStdin<T>(string stdin, Func<T> func)
        {
            var originalIn = Console.In;
            try
            {
                Console.SetIn(new StringReader(stdin));
                return func();
            }
            finally
            {
                Console.SetIn(originalIn);
            }
        }
        #endregion

        /// <summary>
        /// Integer literal evaluates to its integer value.
        /// </summary>
        [Fact]
        public void Evaluate_IntegerLiteral_ReturnsIntegerValue()
        {
            var interp = new Interpreter();
            var node = new IntegerLiteral(1);
            var result = interp.Evaluate(node);

            Assert.IsType<int>(result);
            Assert.Equal(1, result);
        }

        /// <summary>
        /// Float literal evaluates to its double value.
        /// </summary>
        [Fact]
        public void Evaluate_FloatLiteral_ReturnsFloatValue()
        {
            var interp = new Interpreter();
            var node = new FloatLiteral(1.23);
            var result = interp.Evaluate(node);

            Assert.IsType<double>(result);
            Assert.Equal(1.23, (double)result, 2);
        }

        /// <summary>
        /// Addition of two integer returns their sum.
        /// </summary>
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
        
        /// <summary>
        /// Multiplication of two integers returns their products.
        /// </summary>
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

        /// <summary>
        /// Addition between float and int returns a double.
        /// </summary>
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

        /// <summary>
        /// Assignement store value in the interpreter environment.
        /// </summary>
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

        /// <summary>
        /// Referencing a variable returns the stored value.
        /// </summary>
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

        /// <summary>
        /// Blocks execute statements in order and return last expression value.
        /// </summary>
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

        /// <summary>
        /// == returns true for equal values.
        /// </summary>
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

        /// <summary>
        /// != returns true for different value.
        /// </summary>
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

        /// <summary>
        /// Boolean literals evaluate to bool value.
        /// </summary>
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

        /// <summary>
        /// If with boolean condition executes the correct branch.
        /// </summary>
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

        /// <summary>
        /// Less-than comparison returns a boolean.
        /// </summary>
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

        /// <summary>
        /// Logical AND short-circuits to false.
        /// </summary>
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

        /// <summary>
        /// Logical OR short-circuits to true.
        /// </summary>
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

        /// <summary>
        /// ++ increments an integer variable.
        /// </summary>
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

        /// <summary>
        /// -- decrements an integer variable.
        /// </summary>
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

        /// <summary>
        /// print(...) writes argument value to Console.
        /// </summary>
        [Fact]
        public void Evaluate_FunctionCall_Print_WritesOutput()
        {
            var interp = new Interpreter();
            var functionCall = new FunctionCall("print", new List<Expression> { new IntegerLiteral(5) });
            var output = CaptureOut(() => interp.Evaluate(functionCall));

            Assert.Equal("5", output);
        }

        /// <summary>
        /// Function declaration adds entry to environment.
        /// </summary>
        [Fact]
        public void Evaluate_FunctionDeclaration_AddsFunctionToEnvironment()
        {
            var interp = new Interpreter();
            var funct = new FunctionDeclaration("myFunc", new List<string>(), new Block(new List<Statement>()));
            var result = interp.Evaluate(funct);

            Assert.Null(result);
            Assert.True(interp.GetEnvironment().ContainsKey("myFunc"));
            Assert.IsType<FunctionDeclaration>(interp.GetEnvironment()["myFunc"]);
        }

        /// <summary>
        /// User-defined function executes and can print inside.
        /// </summary>
        [Fact]
        public void Evaluate_FunctionCall_UserFunction_ExecutesFunction()
        {
            var interp = new Interpreter();
            var funct = new FunctionDeclaration(
                "addOne",
                new List<string> { "x" },
                new Block(new List<Statement>
                {
                    new ExpressionStatement(new FunctionCall("print", new List<Expression> { new IntegerLiteral(999) })),
                    new ExpressionStatement(new VariableReference("x"))
                })
            );
            interp.Evaluate(funct);
            var output = CaptureOut(() =>
            {
                var _ = interp.Evaluate(new FunctionCall("addOne", new List<Expression> { new IntegerLiteral(5) }));
            });

            Assert.Equal("999", output);

        }

        /// <summary>
        /// Function can return a value via return statement.
        /// </summary>
        [Fact]
        public void Evaluate_Function_ReturnStatement_ReturnsValue()
        {
            var interp = new Interpreter();
            var function = new FunctionDeclaration("testFunc", new List<string>(), new Block(new List<Statement>
            {
                new ReturnStatement(new IntegerLiteral(99))
            }));
            interp.Evaluate(function);
            var call = new FunctionCall("testFunc", new List<Expression>());
            var result = interp.Evaluate(call);

            Assert.Equal(99, result);
        }

        /// <summary>
        /// Function arguments flow into return expression.
        /// </summary>
        [Fact]
        public void Evaluate_FunctionCall_WithReturn_ReturnsCorrectValue()
        {
            var interp = new Interpreter();
            var funct = new FunctionDeclaration(
                "double",
                new List<string> { "x" },
                new Block(new List<Statement>
                {
                    new ReturnStatement(new BinaryExpression(
                        new VariableReference("x"),
                        OperatorType.Multiply,
                        new FloatLiteral(2.5)
                    ))
                })
            );
            interp.Evaluate(funct);
            var result = interp.Evaluate(new FunctionCall("double", new List<Expression> { new IntegerLiteral(4) }));

            Assert.Equal(10.0, result);
        }

        /// <summary>
        /// Execution stops after the first return in a function.
        /// </summary>
        [Fact]
        public void Evaluate_FunctionCall_Returns_StopsExecutionAfterReturn()
        {
            var interp = new Interpreter();
            var funct = new FunctionDeclaration(
                "earlyReturn",
                new List<string> { },
                new Block(new List<Statement>
                {
                    new ReturnStatement(new IntegerLiteral(10)),
                    new ExpressionStatement(new FunctionCall("print", new List<Expression> { new IntegerLiteral(999) }))
                })
            );
            interp.Evaluate(funct);
            string output = CaptureOut(() =>
            {
                var _ = interp.Evaluate(new FunctionCall("earlyReturn", new List<Expression> { }));
            });

            Assert.Equal("", output);
        }

        /// <summary>
        /// String literal evaluates to its value.
        /// </summary>
        [Fact]
        public void Evaluate_StringLiteral_ReturnsString()
        {
            var interp = new Interpreter();
            var str = new StringLiteral("test123");
            var result = interp.Evaluate(str);

            Assert.Equal("test123", result);
        }

        /// <summary>
        /// String + String concatenation.
        /// </summary>
        [Fact]
        public void Evaluate_BinaryExpression_StringConcat_ReturnsConcatenatedString()
        {
            var interp = new Interpreter();
            var left = new StringLiteral("Hello ");
            var right = new StringLiteral("World");
            var expr = new BinaryExpression(left, OperatorType.Plus, right);
            var result = interp.Evaluate(expr);

            Assert.Equal("Hello World", result);
        }

        /// <summary>
        /// String + Int concatenation.
        /// </summary>
        [Fact]
        public void Evaluate_BinaryExpression_StringAndIntConcat_ReturnsConcatenatedString()
        {
            var interp = new Interpreter();
            var left = new StringLiteral("Value: ");
            var right = new IntegerLiteral(10);
            var expr = new BinaryExpression(left, OperatorType.Plus, right);
            var result = interp.Evaluate(expr);

            Assert.Equal("Value: 10", result);
        }

        /// <summary>
        ///  Int + String concatenation.
        /// </summary>
        [Fact]
        public void Evaluate_BinaryExpression_IntAndStringCOncat_ReturnsConcatenatedString()
        {
            var interp = new Interpreter();
            var left = new IntegerLiteral(100);
            var right = new StringLiteral(" dogs");
            var expr = new BinaryExpression(left, OperatorType.Plus, right);
            var result = interp.Evaluate(expr);

            Assert.Equal("100 dogs", result);
        }

        /// <summary>
        /// input() redas from stdin, returning parsed int or raw string.
        /// </summary>
        [Fact]
        public void Evaluate_FunctionCall_Input_ReadsUserInput()
        {
            var interp = new Interpreter();
            object? result1 = null;
            CaptureOut(() =>
            {
                result1 = WithStdin("123\n", () =>
                    interp.Evaluate(new FunctionCall("input", new List<Expression>()))
                );
            });

            Assert.Equal(123, result1);

            object? result2 = null;
            CaptureOut(() =>
            {
                result2 = WithStdin("hello world\n", () =>
                    interp.Evaluate(new FunctionCall("input", new List<Expression>()))
                );
            });

            Assert.Equal("hello world", result2);
        }

        /// <summary>
        /// Modulo (%) over integers.
        /// </summary>
        [Fact]
        public void Evaluate_Modulo_ReturnsCorrectResult()
        {
            var interp = new Interpreter();
            var expr = new BinaryExpression(new IntegerLiteral(10), OperatorType.Modulo, new IntegerLiteral(3));

            Assert.Equal(1, interp.Evaluate(expr));
        }

        /// <summary>
        /// Logical NOT (!).
        /// </summary>
        [Fact]
        public void Evaluate_UnaryNot_ReturnsCorrectResult()
        {
            var interp = new Interpreter();
            var expr = new UnaryNotExpression(new BooleanLiteral(true));

            Assert.False((bool)interp.Evaluate(expr));
        }

        /// <summary>
        /// Bitwise AND (&amp;).
        /// </summary>
        [Fact]
        public void Evaluate_BitwiseAnd_ReturnsCorrectResult()
        {
            var interp = new Interpreter();
            var expr = new BinaryExpression(new IntegerLiteral(6), OperatorType.BitwiseAnd, new IntegerLiteral(3));

            Assert.Equal(2, interp.Evaluate(expr));
        }

        /// <summary>
        /// Bitwise OR (|)
        /// </summary>
        [Fact]
        public void Evaluate_BitwiseOr_ReturnsCorrectResult()
        {
            var interp = new Interpreter();
            var expr = new BinaryExpression(new IntegerLiteral(6), OperatorType.BitwiseOr, new IntegerLiteral(3));

            Assert.Equal(7, interp.Evaluate(expr));
        }

        /// <summary>
        /// Array literal evaluates to a List&lt;object&gt;.
        /// </summary>
        [Fact]
        public void Evaluate_ArrayLiteral_ReturnsList()
        {
            var interp = new Interpreter();
            var array = new ArrayLiteral(new List<Expression> { new IntegerLiteral(1), new IntegerLiteral(2), new IntegerLiteral(3) });
            var result = interp.Evaluate(array);

            Assert.IsType<List<object>>(result);
            Assert.Equal(3, ((List<object>)result).Count);
        }

        /// <summary>
        /// Array access returns the correct element.
        /// </summary>
        [Fact]
        public void Evaluate_ArrayAccess_ReturnsCorrectElement()
        {
            var interp = new Interpreter();
            var assign = new Assignment("a", new ArrayLiteral(new List<Expression> { new IntegerLiteral(10), new IntegerLiteral(20) }));
            interp.Evaluate(assign);
            var access = new ArrayAccess("a", new IntegerLiteral(1));
            var result = interp.Evaluate(access);

            Assert.Equal(20, result);
        }

        /// <summary>
        /// Array assignment updates the element at index;
        /// </summary>
        [Fact]
        public void Evaluate_ArrayAssignment_UpdatesArrayElement()
        {
            var interp = new Interpreter();
            var assign = new Assignment("a", new ArrayLiteral(new List<Expression> { new IntegerLiteral(10), new IntegerLiteral(20) }));
            interp.Evaluate(assign);
            var arrayAssign = new ArrayAssignment("a", new IntegerLiteral(0), new IntegerLiteral(99));
            interp.Evaluate(arrayAssign);
            var access = new ArrayAccess("a", new IntegerLiteral(0));
            var result = interp.Evaluate(access);

            Assert.Equal(99, result);
        }

        /// <summary>
        /// null literal returns null.
        /// </summary>
        [Fact]
        public void Evaluate_NullLiteral_ReturnsNull()
        {
            var interp = new Interpreter();
            var result = interp.Evaluate(new NullLiteral());

            Assert.Null(result);
        }

        /// <summary>
        /// break exits a while loop early.
        /// </summary>
        [Fact]
        public void Interpreter_Break_ExitsLoopEarly()
        {
            var interp = new Interpreter();
            var node = new Block(new List<Statement>
            {
                new Assignment("i", new IntegerLiteral(0)),
                new WhileStatement(
                    new BinaryExpression(new VariableReference("i"), OperatorType.Less, new IntegerLiteral(5)),
                    new Block(new List<Statement>
                    {
                        new IfStatement(
                            new BinaryExpression(new VariableReference("i"), OperatorType.Equalequal, new IntegerLiteral(2)),
                            new BreakStatement()
                        ),
                        new Assignment("i", new BinaryExpression(new VariableReference("i"), OperatorType.Plus, new IntegerLiteral(1)))
                    })
                )
            });
            interp.Evaluate(node);

            Assert.Equal(2, interp.GetEnvironment()["i"]);
        }

        /// <summary>
        /// continue skips the rest of the current iteration.
        /// </summary>
        [Fact]
        public void Interpreter_Continue_SkipsCurrentIteration()
        {
            var interp = new Interpreter();
            var node = new Block(new List<Statement>
            {
                new Assignment("i", new IntegerLiteral(0)),
                new Assignment("sum", new IntegerLiteral(0)),
                new WhileStatement(
                    new BinaryExpression(new VariableReference("i"), OperatorType.Less, new IntegerLiteral(5)),
                    new Block(new List<Statement>
                    {
                        new Assignment("i", new BinaryExpression(new VariableReference("i"), OperatorType.Plus, new IntegerLiteral(1))),
                        new IfStatement(
                            new BinaryExpression(new VariableReference("i"), OperatorType.Equalequal, new IntegerLiteral(3)),
                            new ContinueStatement()
                        ),
                        new Assignment("sum", new BinaryExpression(new VariableReference("sum"), OperatorType.Plus, new VariableReference("i")))
                    })
                )
            });
            interp.Evaluate(node);

            Assert.Equal(1 + 2 + 4 + 5, interp.GetEnvironment()["sum"]);
        }

        [Fact]
        public void Interpreter_Can_DeclareAndAssignVariale()
        {
            var interp = new Interpreter();
            var statements = new List<Statement>
            {
                new VariableDeclaration("x", new IntegerLiteral(5), null),
                new Assignment("x", new IntegerLiteral(10)),
                new ExpressionStatement(new VariableReference("x"))
            };
            var result = interp.Evaluate(new Block(statements));

            Assert.Equal(10, result);
        }

        /// <summary>
        /// Redeclaring a variable throws a runtime error.
        /// </summary>
        [Fact]
        public void Interpreter_Throws_OnRedeclarationOfVariable()
        {
            var interp = new Interpreter();
            var statements = new List<Statement>
            {
                new VariableDeclaration("x", new IntegerLiteral(5), null),
                new VariableDeclaration("x", new IntegerLiteral(10), null)
            };
            
            Assert.Throws<RuntimeException>(() => interp.Evaluate(new Block(statements)));
        }

        /// <summary>
        /// Constants can be declared and read.
        /// </summary>
        [Fact]
        public void Interpreter_CanDeclareAndUseConstant()
        {
            var interp = new Interpreter();
            var statements = new List<Statement>
            {
                new ConstantDeclaration("y", new IntegerLiteral(10), null),
                new ExpressionStatement(new VariableReference("y"))
            };
            var result = interp.Evaluate(new Block(statements));

            Assert.Equal(10, result);
        }

        /// <summary>
        /// Redeclaring a constant throws a runtime error.
        /// </summary>
        [Fact]
        public void Interpreter_Throws_OnRedeclarationOfConstant()
        {
            var interp = new Interpreter();
            var statement = new List<Statement>
            {
                new ConstantDeclaration("y", new IntegerLiteral(1), null),
                new ConstantDeclaration("y", new IntegerLiteral(2), null)
            };

            Assert.Throws<RuntimeException>(() => interp.Evaluate(new Block(statement)));
        }

        /// <summary>
        /// Assigning to a constant throws a runtime error.
        /// </summary>
        [Fact]
        public void Interpreter_Throws_OnAssignmentToConstant()
        {
            var interp = new Interpreter();
            var statements = new List<Statement>
            {
                new ConstantDeclaration("y", new IntegerLiteral(5), null),
                new Assignment("y", new IntegerLiteral(10))
            };

            Assert.Throws<RuntimeException>(() => interp.Evaluate(new Block(statements)));
        }

        /// <summary>
        /// Type annotations are honored when correct.
        /// </summary>
        [Fact]
        public void Interpreter_RespectsTypeAnnotation_CorrectType_DoesNotThrow()
        {
            var interp = new Interpreter();
            var block = new Block(new List<Statement>
            {
                new VariableDeclaration("a", new IntegerLiteral(10), new TypeAnnotation("int", TokenType.TypeInt)),
                new VariableDeclaration("b", new FloatLiteral(1.5), new TypeAnnotation("float", TokenType.TypeFloat)),
                new VariableDeclaration("c", new StringLiteral("hello"), new TypeAnnotation("string", TokenType.TypeString)),
                new VariableDeclaration("d", new BooleanLiteral(true), new TypeAnnotation("bool", TokenType.TypeBool))
            });
            interp.Evaluate(block);

            Assert.Equal(10, interp.GetEnvironment()["a"]);
            Assert.Equal(1.5, interp.GetEnvironment()["b"]);
            Assert.Equal("hello", interp.GetEnvironment()["c"]);
            Assert.Equal(true, interp.GetEnvironment()["d"]);
        }

        /// <summary>
        /// Type annotations reject incompatibles values.
        /// </summary>
        [Fact]
        public void Interpreter_RespectsTypeAnnotation_WrongType_ThrowsException()
        {
            var interp = new Interpreter();
            var block = new Block(new List<Statement>
            {
                new VariableDeclaration("a", new StringLiteral("notAnInt"), new TypeAnnotation("int", TokenType.TypeInt))
            });

            Assert.Throws<RuntimeException>(() => interp.Evaluate(block));
        }

        /// <summary>
        /// Constant declaration with correct type works.
        /// </summary>
        [Fact]
        public void Interpreter_ConstantDeclaration_WithCorrectType_Works()
        {
            var interp = new Interpreter();
            var block = new Block(new List<Statement>
            {
                new ConstantDeclaration("pi", new FloatLiteral(3.14), new TypeAnnotation("float", TokenType.TypeFloat))
            });
            interp.Evaluate(block);

            Assert.Equal(3.14, interp.GetEnvironment()["pi"]);
        }

        /// <summary>
        /// Assignment after typed declaration enforces type compatibility.
        /// </summary>
        [Fact]
        public void Interpreter_Assignment_AfterTypeDeclaration_WithWrongType_Throws()
        {
            var interp = new Interpreter();
            var block = new Block(new List<Statement>
            {
                new VariableDeclaration("x", new IntegerLiteral(1), new TypeAnnotation("int", TokenType.TypeInt)),
                new Assignment("x", new StringLiteral("hello"))
            });

            Assert.Throws<RuntimeException>(() => interp.Evaluate(block));
        }
    }
}