using MinImpLangComp.Lexing;
using MinImpLangComp.Parsing;
using MinImpLangComp.AST;
using Xunit;

namespace MinImpLangComp.Tests
{
    /// <summary>
    /// Expression & statement parsing tests for the recursive-descent parser.
    /// Focuses on literals, operators, precedence/parenthses, unary ops, function declaration/calls, control tokens and typed declarations.
    /// </summary>
    public class ParserTests
    {
        #region Helper
        /// <summary>
        /// Convenience helper to build a parser from source text.
        /// </summary>
        private static Parser MakeParser(string src) => new Parser(new Lexer(src));
        #endregion

        /// <summary>
        /// Parses a single integer literal.
        /// </summary>
        [Fact]
        public void ParseExpression_SingleInteger_ReturnsIntegerLiteral()
        {
            var parser = MakeParser("12");
            var expr = parser.ParseExpression();

            var intLiteral = Assert.IsType<IntegerLiteral>(expr);
            Assert.Equal(12, intLiteral.Value);
        }

        /// <summary>
        /// Parse a single float literal.
        /// </summary>
        [Fact]
        public void ParseExpression_SingleFloat_ReturnsFloatLiteral()
        {
            var parser = MakeParser("3.12");
            var expr = parser.ParseExpression();

            var floatLiteral = Assert.IsType<FloatLiteral>(expr);
            Assert.Equal(3.12, floatLiteral.Value, 3);
        }

        /// <summary>
        /// Parses a simple addition and returns a binary expression AST.
        /// </summary>
        [Fact]
        public void ParseExpression_Addition_ReturnsBinaryExpression()
        {
            var parser = MakeParser("1 + 2");
            var expr = parser.ParseExpression();

            var binary = Assert.IsType<BinaryExpression>(expr);
            Assert.Equal(OperatorType.Plus, binary.Operator);
            var left = Assert.IsType<IntegerLiteral>(binary.Left);
            Assert.Equal(1, left.Value);
            var right = Assert.IsType<IntegerLiteral>(binary.Right);
            Assert.Equal(2, right.Value);
        }

        /// <summary>
        /// Checks operator precedence: multiplication binds tighter than addition.
        /// </summary>
        [Fact]
        public void ParseExpression_MultiplicationWithPrecedence_ReturnsCorrectAST()
        {
            var parser = MakeParser("1 + 2 * 3");
            var expr = parser.ParseExpression();

            var binary = Assert.IsType<BinaryExpression>(expr);
            var left = Assert.IsType<IntegerLiteral>(binary.Left);
            Assert.Equal(1, left.Value);
            var right = Assert.IsType<BinaryExpression>(binary.Right);
            Assert.Equal(OperatorType.Multiply, right.Operator);
            var rightL = Assert.IsType<IntegerLiteral> (right.Left);
            Assert.Equal (2, rightL.Value);
            var rightR = Assert.IsType<IntegerLiteral> (right.Right);
            Assert.Equal(3, rightR.Value);
        }

        /// <summary>
        /// Parentheses override precedence: (1+2)*3.
        /// </summary>
        [Fact]
        public void ParseExpression_ParentheseOverridePrecedence_ReturnsCorrectAST()
        {
            var parser = MakeParser("(1 + 2) * 3");
            var expr = parser.ParseExpression();

            var binary = Assert.IsType<BinaryExpression>(expr);
            Assert.Equal(OperatorType.Multiply, binary.Operator);
            var left = Assert.IsType<BinaryExpression>(binary.Left);
            Assert.Equal(OperatorType.Plus, left.Operator);
            var leftL = Assert.IsType<IntegerLiteral>(left.Left);
            Assert.Equal(1, leftL.Value);
            var leftR = Assert.IsType <IntegerLiteral> (left.Right);
            Assert.Equal(2, leftR.Value);
            var right = Assert.IsType<IntegerLiteral>(binary.Right);
            Assert.Equal(3, right.Value);
        }
        
        /// <summary>
        /// PArses equality operator (==) into a binary expression.
        /// </summary>
        [Fact]
        public void ParseExpression_Equality_ReturnsBinaryExpression()
        {
            var parser = MakeParser("5 == 5");
            var expr = parser.ParseExpression();

            var binary = Assert.IsType<BinaryExpression> (expr);
            Assert.Equal(OperatorType.Equalequal, binary.Operator);
            var left = Assert.IsType<IntegerLiteral> (binary.Left);
            Assert.Equal(5, left.Value);
            var right = Assert.IsType<IntegerLiteral>(binary.Right);
            Assert.Equal(5, right.Value);
        }

        /// <summary>
        /// Parses inequality operator (!=) into a binary expression.
        /// </summary>
        [Fact]
        public void ParseExpression_NotEqual_ReturnsBinaryExpression()
        {
            var parser = MakeParser("3 != 4");
            var expr = parser.ParseExpression();

            var binary = Assert.IsType<BinaryExpression>(expr);
            Assert.Equal(OperatorType.NotEqual, binary.Operator);
            var left = Assert.IsType<IntegerLiteral>(binary.Left);
            Assert.Equal(3, left.Value);
            var right = Assert.IsType<IntegerLiteral>(binary.Right);
            Assert.Equal(4, right.Value);
        }

        /// <summary>
        /// Parses boolean literal 'true'.
        /// </summary>
        [Fact]
        public void ParseExpression_BooleanLiteral_True()
        {
            var parser = MakeParser("true");
            var expr = parser.ParseExpression();

            var boolLiteral = Assert.IsType<BooleanLiteral> (expr);
            Assert.True(boolLiteral.Value);
        }

        [Fact]
        public void ParseExpression_BooleanLiteral_False()
        {
            var parser = MakeParser("false");
            var expr = parser.ParseExpression();
            
            var boolLiteral = Assert.IsType<BooleanLiteral>(expr);
            Assert.False(boolLiteral.Value);
        }

        /// <summary>
        /// Parses logical AND (&amp;&amp;) with boolean operands.
        /// </summary>
        [Fact]
        public void ParseExpression_LogicalAnd_ReturnsBinaryExpression()
        {
            var parser = MakeParser("true && false");
            var expr = parser.ParseExpression();
            
            var binary = Assert.IsType<BinaryExpression>(expr);
            Assert.Equal(OperatorType.AndAnd, binary.Operator);
            Assert.IsType<BooleanLiteral>(binary.Left);
            Assert.IsType<BooleanLiteral>(binary.Right);
        }

        /// <summary>
        /// Parses logical OR (||) with boolean operands.
        /// </summary>
        [Fact]
        public void ParseExpresion_LogicalOr_ReturnsBinaryExpression()
        {
            var parser = MakeParser("true || false");
            var expr = parser.ParseExpression();

            var binary = Assert.IsType<BinaryExpression>(expr);
            Assert.Equal(OperatorType.OrOr, binary.Operator);
            Assert.IsType<BooleanLiteral>(binary.Left);
            Assert.IsType<BooleanLiteral>(binary.Right);
        }

        /// <summary>
        /// Parses prefix increment statement into a unary expression.
        /// </summary>
        [Fact]
        public void ParseStatement_UnaryIncrement_ReturnsUnaryExpression()
        {
            var parser = MakeParser("++ x;");
            var statement = parser.ParseStatement();

            var exprSt = Assert.IsType<ExpressionStatement>(statement);
            var unary = Assert.IsType<UnaryExpression>(exprSt.Expression);
            Assert.Equal(OperatorType.PlusPlus, unary.Operator);
            Assert.Equal("x", unary.Identifier);
        }

        /// <summary>
        /// Parses a function declaration without parameters.
        /// </summary>
        [Fact]
        public void ParseStatement_UnaryDecrement_ReturnsUnaryExpression()
        {
            var parser = MakeParser("-- y;");
            var statement = parser.ParseStatement();

            var exprSt = Assert.IsType<ExpressionStatement>(statement);
            var unary = Assert.IsType<UnaryExpression>(exprSt.Expression);
            Assert.Equal(OperatorType.MinusMinus, unary.Operator);
            Assert.Equal("y", unary.Identifier);
        }

        /// <summary>
        /// Parses a function declaration without parameters.
        /// </summary>
        [Fact]
        public void ParseFunctionDeclaration_WithNoParameters_ReturnsCorrectAST()
        {
            var parser = MakeParser("function myFunc() { set x = 5; }");
            var statement = parser.ParseStatement();
            
            var functDecla = Assert.IsType<FunctionDeclaration>(statement);
            Assert.Equal("myFunc", functDecla.Name);
            Assert.Empty(functDecla.Parameters);
            Assert.Single(functDecla.Body.Statements);
        }

        /// <summary>
        /// Parses a function declaration with parameters and a simple body.
        /// </summary>
        [Fact]
        public void ParseFunctionDeclaration_WithParamaters_ReturnsCorrectAST()
        {
            var parser = MakeParser("function add(a, b) { set result = a + b; }");
            var statement = parser.ParseStatement();

            var functDecla = Assert.IsType<FunctionDeclaration>(statement);
            Assert.Equal("add", functDecla.Name);
            Assert.Equal(2, functDecla.Parameters.Count);
            Assert.Contains("a", functDecla.Parameters);
            Assert.Contains("b", functDecla.Parameters);
            Assert.Single(functDecla.Body.Statements);
        }

        /// <summary>
        /// Parses a string literal factor.
        /// </summary>
        [Fact]
        public void ParseFactor_StringLiteral_ReturnsStringLiteral()
        {
            var parser = MakeParser("\"abc\"");
            var expr = parser.ParseExpression();

            var stringLiteral = Assert.IsType<StringLiteral>(expr);
            Assert.Equal("abc", stringLiteral.Value);
        }

        /// <summary>
        /// Parses modulo (%) as a binary operator in an expression statement.
        /// </summary>
        [Fact]
        public void ParseExpression_Modulo_ReturnsBinaryExpression()
        {
            var parser = MakeParser("5 % 2;");
            var statement = parser.ParseStatement();

            var exprStmt = Assert.IsType<ExpressionStatement>(statement);
            var binExpr = Assert.IsType<BinaryExpression>(exprStmt.Expression);
            Assert.Equal(OperatorType.Modulo, binExpr.Operator);
        }

        /// <summary>
        /// Parses unary logical NOT into a <see cref="UnaryNotExpression"/>.
        /// </summary>
        [Fact]
        public void ParseExpression_UnaryNot_ReturnsUnaryNotExpression()
        {
            var parser = MakeParser("!true;");
            var statement = parser.ParseStatement();
            
            var exprStmt = Assert.IsType<ExpressionStatement>(statement);
            var notExpr = Assert.IsType<UnaryNotExpression>(exprStmt.Expression);
        }

        /// <summary>
        /// Parses bitwise AND (&amp;) as a binary operator.
        /// </summary>
        [Fact]
        public void ParseExpression_BitwiseAnd_ReturnsBinaryExpression()
        {
            var parser = MakeParser("5 & 3;");
            var statement = parser.ParseStatement();

            var exprStmt = Assert.IsType<ExpressionStatement>(statement);
            var binExpr = Assert.IsType<BinaryExpression>(exprStmt.Expression);
            Assert.Equal(OperatorType.BitwiseAnd, binExpr.Operator);
        }

        /// <summary>
        /// Parses bitwise OR (|) as a binary operator.
        /// </summary>
        [Fact]
        public void ParseExpresion_BitwiseOr_ReturnsBinaryExpression()
        {
            var parser = MakeParser("5 | 3;");
            var statement = parser.ParseStatement();

            var exprStmt = Assert.IsType<ExpressionStatement>(statement);
            var binExpr = Assert.IsType<BinaryExpression>(exprStmt.Expression);
            Assert.Equal(OperatorType.BitwiseOr, binExpr.Operator);
        }

        /// <summary>
        /// Parses a function call whose argument itself an expression.
        /// </summary>
        [Fact]
        public void Parser_CanParseFunctionCallWithExpressionArgument()
        {
            var parser = MakeParser("print(a + 5);");
            var statement = parser.ParseStatement();

            Assert.IsType<ExpressionStatement>(statement);

            var exprStmt = (ExpressionStatement)statement;
            Assert.IsType<FunctionCall>(exprStmt.Expression);
        }

        /// <summary>
        /// Parses the null literal as an expression statement.
        /// </summary>
        [Fact]
        public void Parser_CanParseNullLiteral()
        {
            var parser = MakeParser("null;");
            var statement = parser.ParseStatement();

            var exprStmt = Assert.IsType<ExpressionStatement>(statement);
            Assert.IsType<NullLiteral>(exprStmt.Expression);
        }

        /// <summary>
        /// Parses <c>break;</c> into a <see cref="BreakStatement"/>.
        /// </summary>
        [Fact]
        public void Parser_CanParseBreakStatetement()
        {
            var parser = MakeParser("break;");
            var statement = parser.ParseStatement();

            Assert.IsType<BreakStatement>(statement);
        }

        /// <summary>
        /// Parses <c>continue;</c> into a <see cref="ContinueStatement"/>.
        /// </summary>
        [Fact]
        public void Parser_CanParseContinueStatement()
        {
            var parser = MakeParser("continue;");
            var statement = parser.ParseStatement();

            Assert.IsType<ContinueStatement>(statement);
        }

        /// <summary>
        /// Parses a <c>set</c> declaration as a <see cref="VariableDeclaration"/>.
        /// </summary>
        [Fact]
        public void Parser_CanParseSetStatementAsVariableDeclaration()
        {
            var parser = MakeParser("set x = 42;");
            var statement = parser.ParseStatement();

            var assign = Assert.IsType<VariableDeclaration>(statement);
            Assert.Equal("x", assign.Identifier);

            var literal = Assert.IsType<IntegerLiteral>(assign.Expression);
            Assert.Equal(42, literal.Value);
        }

        /// <summary>
        /// Parses a <c>bind</c> declaration as a <see cref="ConstantDeclaration"/>.
        /// </summary>
        [Fact]
        public void Parser_ShouldReturnConstantDeclaration_WhenParsingBindStatement()
        {
            var parser = MakeParser("bind pi = 3.14;");
            var statement = parser.ParseStatement();

            var assign = Assert.IsType<ConstantDeclaration>(statement);
            Assert.Equal("pi", assign.Identifier);

            var literal = Assert.IsType<FloatLiteral>(assign.Expression);
            Assert.Equal(3.14, literal.Value, 3);
        }

        /// <summary>
        /// Parses typed <c>set</c> with a type annotation (e.g., <c>: int</c>).
        /// </summary>
        [Fact]
        public void Parser_CanParseSetStatementWithTypeAnnotation()
        {
            var parser = MakeParser("set x: int = 10;");
            var statement = parser.ParseStatement();

            var decl = Assert.IsType<VariableDeclaration>(statement);
            Assert.Equal("x", decl.Identifier);
            var literal = Assert.IsType<IntegerLiteral>(decl.Expression);
            Assert.Equal(10, literal.Value);
            Assert.NotNull(decl.TypeAnnotation);
            Assert.Equal("int", decl.TypeAnnotation.TypeName);
            Assert.Equal(TokenType.TypeInt, decl.TypeAnnotation.TypeToken);
        }

        /// <summary>
        /// Parses typed <c>bind</c> with a boolean type annotation.
        /// </summary>
        [Fact]
        public void Parser_CanParseBindStatementWithTypeAnnotation()
        {
            var parser = MakeParser("bind flag: bool = true;");
            var statement = parser.ParseStatement();

            var decl = Assert.IsType<ConstantDeclaration>(statement);
            Assert.Equal("flag", decl.Identifier);
            var literal = Assert.IsType<BooleanLiteral>(decl.Expression);
            Assert.True(literal.Value);
            Assert.NotNull(decl.TypeAnnotation);
            Assert.Equal("bool", decl.TypeAnnotation.TypeName);
            Assert.Equal(TokenType.TypeBool, decl.TypeAnnotation.TypeToken);
        }

        /// <summary>
        /// Parses <c>set</c> without a type annotation; annotation should be null.
        /// </summary>
        [Fact]
        public void Parser_SetStatementWithoutTypeAnnotation_HasNullType()
        {
            var parser = MakeParser("set y = 20;");
            var statement = parser.ParseStatement();

            var decl = Assert.IsType<VariableDeclaration>(statement);
            Assert.Equal("y", decl.Identifier);
            var literal = Assert.IsType<IntegerLiteral>(decl.Expression);
            Assert.Equal(20, literal.Value);
            Assert.Null(decl.TypeAnnotation);
        }
    }
}
