using MinImpLangComp.Lexing;
using MinImpLangComp.Parsing;
using MinImpLangComp.AST;
using Xunit;
using System.Reflection.Emit;


namespace MinImpLangComp.Tests
{
    public class ParserTests
    {
        [Fact]
        public void ParseExpression_SingleInteger_ReturnsIntegerLiteral()
        {
            var lexer = new Lexer("12");
            var parser = new Parser(lexer);
            var expr = parser.ParseExpression();

            var intLiteral = Assert.IsType<IntegerLiteral>(expr);
            Assert.Equal(12, intLiteral.Value);
        }

        [Fact]
        public void ParseExpression_SingleFloat_ReturnsFloatLiteral()
        {
            var lexer = new Lexer("3.12");
            var parser = new Parser(lexer);
            var expr = parser.ParseExpression();

            var floatLiteral = Assert.IsType<FloatLiteral>(expr);
            Assert.Equal(3.12, floatLiteral.Value, 3);
        }

        [Fact]
        public void ParseExpression_Addition_ReturnsBinaryExpression()
        {
            var lexer = new Lexer("1 + 2");
            var parser = new Parser(lexer);
            var expr = parser.ParseExpression();

            var binary = Assert.IsType<BinaryExpression>(expr);
            Assert.Equal(OperatorType.Plus, binary.Operator);
            var left = Assert.IsType<IntegerLiteral>(binary.Left);
            Assert.Equal(1, left.Value);
            var right = Assert.IsType<IntegerLiteral>(binary.Right);
            Assert.Equal(2, right.Value);
        }

        [Fact]
        public void ParseExpression_MultiplicationWithPrecedence_ReturnsCorrectAST()
        {
            var lexer = new Lexer("1 + 2 * 3");
            var parser = new Parser(lexer);
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

        [Fact]
        public void ParseExpression_ParentheseOverridePrecedence_ReturnsCorrectAST()
        {
            var lexer = new Lexer("(1 + 2) * 3");
            var parser = new Parser(lexer);
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

        [Fact]
        public void ParseExpression_Equality_ReturnsBinaryExpression()
        {
            var lexer = new Lexer("5 == 5");
            var parser = new Parser(lexer);
            var expr = parser.ParseExpression();

            var binary = Assert.IsType<BinaryExpression> (expr);
            Assert.Equal(OperatorType.Equalequal, binary.Operator);
            var left = Assert.IsType<IntegerLiteral> (binary.Left);
            Assert.Equal(5, left.Value);
            var right = Assert.IsType<IntegerLiteral>(binary.Right);
            Assert.Equal(5, right.Value);
        }

        [Fact]
        public void ParseExpression_NotEqual_ReturnsBinaryExpression()
        {
            var lexer = new Lexer("3 != 4");
            var parser = new Parser(lexer);
            var expr = parser.ParseExpression();

            var binary = Assert.IsType<BinaryExpression>(expr);
            Assert.Equal(OperatorType.NotEqual, binary.Operator);
            var left = Assert.IsType<IntegerLiteral>(binary.Left);
            Assert.Equal(3, left.Value);
            var right = Assert.IsType<IntegerLiteral>(binary.Right);
            Assert.Equal(4, right.Value);
        }

        [Fact]
        public void ParseExpression_BooleanLiteral_True()
        {
            var lexer = new Lexer("true");
            var parser = new Parser(lexer);
            var expr = parser.ParseExpression();

            var boolLiteral = Assert.IsType<BooleanLiteral> (expr);
            Assert.True(boolLiteral.Value);
        }

        [Fact]
        public void ParseExpression_BooleanLiteral_False()
        {
            var lexer = new Lexer("false");
            var parser = new Parser(lexer);
            var expr = parser.ParseExpression();
            
            var boolLiteral = Assert.IsType<BooleanLiteral>(expr);
            Assert.False(boolLiteral.Value);
        }

        [Fact]
        public void ParseExpression_LogicalAnd_ReturnsBinaryExpression()
        {
            var lexer = new Lexer("true && false");
            var parser = new Parser(lexer);
            var expr = parser.ParseExpression();
            
            var binary = Assert.IsType<BinaryExpression>(expr);
            Assert.Equal(OperatorType.AndAnd, binary.Operator);
            Assert.IsType<BooleanLiteral>(binary.Left);
            Assert.IsType<BooleanLiteral>(binary.Right);
        }

        [Fact]
        public void ParseExpresion_LogicalOr_ReturnsBinaryExpression()
        {
            var lexer = new Lexer("true || false");
            var parser = new Parser(lexer);
            var expr = parser.ParseExpression();

            var binary = Assert.IsType<BinaryExpression>(expr);
            Assert.Equal(OperatorType.OrOr, binary.Operator);
            Assert.IsType<BooleanLiteral>(binary.Left);
            Assert.IsType<BooleanLiteral>(binary.Right);
        }

        [Fact]
        public void ParseStatement_UnaryIncrement_ReturnsUnaryExpression()
        {
            var lexer = new Lexer("++ x;");
            var parser = new Parser(lexer);
            var statement = parser.ParseStatement();

            var exprSt = Assert.IsType<ExpressionStatement>(statement);
            var unary = Assert.IsType<UnaryExpression>(exprSt.Expression);
            Assert.Equal(OperatorType.PlusPlus, unary.Operator);
            Assert.Equal("x", unary.Identifier);
        }

        [Fact]
        public void ParseStatement_UnaryDecrement_ReturnsUnaryExpression()
        {
            var lexer = new Lexer("-- y;");
            var parser = new Parser(lexer);
            var statement = parser.ParseStatement();

            var exprSt = Assert.IsType<ExpressionStatement>(statement);
            var unary = Assert.IsType<UnaryExpression>(exprSt.Expression);
            Assert.Equal(OperatorType.MinusMinus, unary.Operator);
            Assert.Equal("y", unary.Identifier);
        }

        [Fact]
        public void ParseFunctionDeclaration_WithNoParameters_ReturnsCorrectAST()
        {
            var lexer = new Lexer("function myFunc() { set x = 5; }");
            var parser = new Parser(lexer);
            var statement = parser.ParseStatement();
            
            var functDecla = Assert.IsType<FunctionDeclaration>(statement);
            Assert.Equal("myFunc", functDecla.Name);
            Assert.Empty(functDecla.Parameters);
            Assert.Single(functDecla.Body.Statements);
        }

        [Fact]
        public void ParseFunctionDeclaration_WithParamaters_ReturnsCorrectAST()
        {
            var lexer = new Lexer("function add(a, b) { set result = a + b; }");
            var parser = new Parser(lexer);
            var statement = parser.ParseStatement();

            var functDecla = Assert.IsType<FunctionDeclaration>(statement);
            Assert.Equal("add", functDecla.Name);
            Assert.Equal(2, functDecla.Parameters.Count);
            Assert.Contains("a", functDecla.Parameters);
            Assert.Contains("b", functDecla.Parameters);
            Assert.Single(functDecla.Body.Statements);
        }

        [Fact]
        public void ParseFactor_StringLiteral_ReturnsStringLiteral()
        {
            var lexer = new Lexer("\"abc\"");
            var parser = new Parser(lexer);
            var expr = parser.ParseExpression();

            var stringLiteral = Assert.IsType<StringLiteral>(expr);
            Assert.Equal("abc", stringLiteral.Value);
        }

        [Fact]
        public void ParseExpression_Modulo_ReturnsBinaryExpression()
        {
            var lexer = new Lexer("5 % 2;");
            var parser = new Parser(lexer);
            var statement = parser.ParseStatement();

            var exprStmt = Assert.IsType<ExpressionStatement>(statement);
            var binExpr = Assert.IsType<BinaryExpression>(exprStmt.Expression);
            Assert.Equal(OperatorType.Modulo, binExpr.Operator);
        }

        [Fact]
        public void ParseExpression_UnaryNot_ReturnsUnaryNotExpression()
        {
            var lexer = new Lexer("!true;");
            var parser = new Parser(lexer);
            var statement = parser.ParseStatement();
            
            var exprStmt = Assert.IsType<ExpressionStatement>(statement);
            var notExpr = Assert.IsType<UnaryNotExpression>(exprStmt.Expression);
        }

        [Fact]
        public void ParseExpression_BitwiseAnd_ReturnsBinaryExpression()
        {
            var lexer = new Lexer("5 & 3;");
            var parser = new Parser(lexer);
            var statement = parser.ParseStatement();

            var exprStmt = Assert.IsType<ExpressionStatement>(statement);
            var binExpr = Assert.IsType<BinaryExpression>(exprStmt.Expression);
            Assert.Equal(OperatorType.BitwiseAnd, binExpr.Operator);
        }

        [Fact]
        public void ParseExpresion_BitwiseOr_ReturnsBinaryExpression()
        {
            var lexer = new Lexer("5 | 3;");
            var parser = new Parser(lexer);
            var statement = parser.ParseStatement();

            var exprStmt = Assert.IsType<ExpressionStatement>(statement);
            var binExpr = Assert.IsType<BinaryExpression>(exprStmt.Expression);
            Assert.Equal(OperatorType.BitwiseOr, binExpr.Operator);
        }

        [Fact]
        public void Parser_CanParseFunctionCallWithExpressionArgument()
        {
            var lexer = new Lexer("print(a + 5);");
            var parser = new Parser(lexer);
            var statement = parser.ParseStatement();

            Assert.IsType<ExpressionStatement>(statement);

            var exprStmt = (ExpressionStatement)statement;
            Assert.IsType<FunctionCall>(exprStmt.Expression);
        }

        [Fact]
        public void Parser_CanParseNullLiteral()
        {
            var lexer = new Lexer("null;");
            var parser = new Parser(lexer);
            var statement = parser.ParseStatement();

            var exprStmt = Assert.IsType<ExpressionStatement>(statement);
            Assert.IsType<NullLiteral>(exprStmt.Expression);
        }

        [Fact]
        public void Parser_CanParseBreakStatetement()
        {
            var lexer = new Lexer("break;");
            var parser = new Parser(lexer);
            var statement = parser.ParseStatement();

            Assert.IsType<BreakStatement>(statement);
        }

        [Fact]
        public void Parser_CanParseContinueStatement()
        {
            var lexer = new Lexer("continue;");
            var parser = new Parser(lexer);
            var statement = parser.ParseStatement();

            Assert.IsType<ContinueStatement>(statement);
        }

        [Fact]
        public void Parser_CanParseSetStatementAsVariableDeclaration()
        {
            var lexer = new Lexer("set x = 42;");
            var parser = new Parser(lexer);
            var statement = parser.ParseStatement();

            var assign = Assert.IsType<VariableDeclaration>(statement);
            Assert.Equal("x", assign.Identifier);

            var literal = Assert.IsType<IntegerLiteral>(assign.Expression);
            Assert.Equal(42, literal.Value);
        }

        [Fact]
        public void Parser_ShouldReturnConstantDeclaration_WhenParsingBindStatement()
        {
            var lexer = new Lexer("bind pi = 3.14;");
            var parser = new Parser(lexer);
            var statement = parser.ParseStatement();

            var assign = Assert.IsType<ConstantDeclaration>(statement);
            Assert.Equal("pi", assign.Identifier);

            var literal = Assert.IsType<FloatLiteral>(assign.Expression);
            Assert.Equal(3.14, literal.Value, 3);
        }
    }
}
