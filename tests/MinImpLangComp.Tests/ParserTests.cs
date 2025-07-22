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
        public void ParseStatement_Print_ReturnsPrintStatement()
        {
            var lexer = new Lexer("print(x);");
            var parser = new Parser(lexer);
            var statement = parser.ParseStatement();

            var printStatement = Assert.IsType<PrintStatement>(statement);
            var variable = Assert.IsType<VariableReference>(printStatement.Expression);
            Assert.Equal("x", variable.Name);
        }
    }
}
