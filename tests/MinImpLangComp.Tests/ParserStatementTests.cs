using MinImpLangComp.LexerSpace;
using MinImpLangComp.ParserSpace;
using MinImpLangComp.AST;
using Xunit;

namespace MinImpLangComp.Tests
{
    public class ParserStatementTests
    {
        [Fact]
        public void ParseStatement_Assignment_ReturnsAssignmentNode()
        {
            var lexer = new Lexer("let x = 1 + 2;");
            var parser = new Parser(lexer);
            var statement = parser.ParseStatement();

            var assignment = Assert.IsType<Assignment>(statement);
            Assert.Equal("x", assignment.Identifier);
            var binary = Assert.IsType<BinaryExpression>(assignment.Expression);
            Assert.Equal("+", binary.Operator);
            var left = Assert.IsType<IntegerLiteral>(binary.Left);
            Assert.Equal(1, left.Value);
            var right = Assert.IsType<IntegerLiteral>(binary.Right);
            Assert.Equal(2, right.Value);
        }

        [Fact]
        public void ParseStatement_BlockEmpty_ReturnsBlockNode()
        {
            var lexer = new Lexer("{ }");
            var parser = new Parser(lexer);
            var block = parser.ParseBlock();

            Assert.Empty(block.Statements);
        }

        [Fact]
        public void ParseStatement_BlockWithMultipleStatements_ReturnsBlockNodewithStatements()
        {
            var input = @"
                    {
                        let a = 1;
                        let b = a + 2; 
                    }                
                ";
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var block = parser.ParseBlock();

            Assert.Equal(2, block.Statements.Count);

            var statement1 = Assert.IsType<Assignment>(block.Statements[0]);
            Assert.Equal("a", statement1.Identifier);
            var express1 = Assert.IsType<IntegerLiteral>(statement1.Expression);
            Assert.Equal(1, express1.Value);
            var statement2 = Assert.IsType<Assignment>(block.Statements[1]);
            Assert.Equal("b", statement2.Identifier);
            var express2 = Assert.IsType<BinaryExpression>(statement2.Expression);
            Assert.Equal("+", express2.Operator);
            var left = Assert.IsType<VariableReference>(express2.Left);
            Assert.Equal("a", left.Name);
            var right = Assert.IsType<IntegerLiteral>(express2.Right);
            Assert.Equal(2, right.Value);
        }
    }
}
