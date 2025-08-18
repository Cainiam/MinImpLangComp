using MinImpLangComp.Lexing;
using MinImpLangComp.Parsing;
using MinImpLangComp.AST;
using Xunit;

namespace MinImpLangComp.Tests
{
    /// <summary>
    /// Parser statement tests: covers variable declarations, empty blocks, and blocks with multiple statements.
    /// </summary>
    public class ParserStatementTests
    {
        /// <summary>
        /// Small helper that wires a <see cref="Parser"/> to a <see cref="Lexer"/> from source text.
        /// </summary>
        private static Parser MakeParser(string source) => new Parser(new Lexer(source));

        /// <summary>
        /// Parses a simple variable declaration with an infix expression on the right-hand side.
        /// Input: <c>setx = 1 + 2;</c>
        /// </summary>
        [Fact]
        public void ParseStatement_Assignment_ReturnsAssignmentNode()
        {
            var parser = MakeParser("set x = 1 + 2;");
            var statement = parser.ParseStatement();

            var assignment = Assert.IsType<VariableDeclaration>(statement);
            Assert.Equal("x", assignment.Identifier);
            var binary = Assert.IsType<BinaryExpression>(assignment.Expression);
            Assert.Equal(OperatorType.Plus, binary.Operator);
            var left = Assert.IsType<IntegerLiteral>(binary.Left);
            Assert.Equal(1, left.Value);
            var right = Assert.IsType<IntegerLiteral>(binary.Right);
            Assert.Equal(2, right.Value);
        }

        /// <summary>
        /// Parses an empty block <c>{ }</c> and verifies it contains no statements.
        /// </summary>
        [Fact]
        public void ParseStatement_BlockEmpty_ReturnsBlockNode()
        {
            var parser = MakeParser("{ }");
            var block = parser.ParseBlock();

            Assert.Empty(block.Statements);
        }

        /// <summary>
        /// Parses a block with two statements and validates their structure.
        /// <code>
        /// {
        ///     set a = 1;
        ///     set b = a + 2;
        /// }
        /// </code>
        /// </summary>
        [Fact]
        public void ParseStatement_BlockWithMultipleStatements_ReturnsBlockNodewithStatements()
        {
            var input = @"
                    {
                        set a = 1;
                        set b = a + 2; 
                    }                
                ";
            var parser = MakeParser(input);
            var block = parser.ParseBlock();

            Assert.Equal(2, block.Statements.Count);

            var statement1 = Assert.IsType<VariableDeclaration>(block.Statements[0]);
            Assert.Equal("a", statement1.Identifier);
            var express1 = Assert.IsType<IntegerLiteral>(statement1.Expression);
            Assert.Equal(1, express1.Value);
            var statement2 = Assert.IsType<VariableDeclaration>(block.Statements[1]);
            Assert.Equal("b", statement2.Identifier);
            var express2 = Assert.IsType<BinaryExpression>(statement2.Expression);
            Assert.Equal(OperatorType.Plus, express2.Operator);
            var left = Assert.IsType<VariableReference>(express2.Left);
            Assert.Equal("a", left.Name);
            var right = Assert.IsType<IntegerLiteral>(express2.Right);
            Assert.Equal(2, right.Value);
        }
    }
}
