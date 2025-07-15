using MinImpLangComp;
using Xunit;

namespace MinImpLangComp.Tests
{
    public class LexerTests
    {
        [Theory]
        [InlineData("10", TokenType.Integer, "10")]
        [InlineData("3.14", TokenType.Float, "3.14")]
        [InlineData(".5", TokenType.Float, ".5")]
        [InlineData("27.", TokenType.Float, "27.")]
        [InlineData("varName", TokenType.Identifier, "varName")]
        [InlineData("+", TokenType.Plus, "+")]
        [InlineData("-", TokenType.Minus, "-")]
        [InlineData("*", TokenType.Multiply, "*")]
        [InlineData("/", TokenType.Divide, "/")]
        [InlineData("=", TokenType.Assign, "=")]
        [InlineData(";", TokenType.Semicolon, ";")]
        [InlineData(".", TokenType.Dot, ".")]
        [InlineData(",", TokenType.Comma, ",")]
        [InlineData("(", TokenType.LeftParen, "(")]
        [InlineData(")", TokenType.RightParen, ")")]
        [InlineData("{", TokenType.LeftBrace, "{")]
        [InlineData("}", TokenType.RightBrace, "}")]
        public void GetNextToken_ReturnsExpecetedToken(string input, TokenType expectedType, string expectedValue)
        {
            var lexer = new Lexer(input);
            var token = lexer.GetNextToken();

            Assert.Equal(expectedType, token.Type);
            Assert.Equal(expectedValue, token.Value);
        }

        [Fact]
        public void GetNextToken_ReturnsEOFAtEnd()
        {
            var lexer = new Lexer("10");
            lexer.GetNextToken();
            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.EOF, token.Type);
        }

        [Fact]
        public void GetNextToken_ReturnsUnknow()
        {
            var lexer = new Lexer("@");
            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.Unknow, token.Type);
            Assert.Equal("@", token.Value);
        }

        [Theory]
        [InlineData("1.2.3", "1.2.3")]
        [InlineData("1..2", "1..2")]
        public void GetNextToken_InvalidNumber_ReturnsLexicalError(string input, string expectedValue)
        {
            var lexer = new Lexer(input);
            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.LexicalError, token.Type);
            Assert.Equal(expectedValue, token.Value);
        }

        [Fact]
        public void GetNextToken_SkipsWhiteSpace()
        {
            var lexer = new Lexer("   50   ");
            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.Integer, token.Type);
            Assert.Equal("50", token.Value);
        }

        [Fact]
        public void GetNextToken_ReadsMultipleTokensInSequence()
        {
            var lexer = new Lexer("var1 = 3 + 4;");
            var tokens = new[]
            {
                lexer.GetNextToken(), //var1
                lexer.GetNextToken(), //=
                lexer.GetNextToken(), //3
                lexer.GetNextToken(), //+
                lexer.GetNextToken(), //4
                lexer.GetNextToken(), //;
                lexer.GetNextToken(), //end
            };

            Assert.Equal(TokenType.Identifier, tokens[0].Type);
            Assert.Equal("var1", tokens[0].Value);

            Assert.Equal(TokenType.Assign, tokens[1].Type);
            Assert.Equal("=", tokens[1].Value);

            Assert.Equal(TokenType.Integer, tokens[2].Type);
            Assert.Equal("3", tokens[2].Value);

            Assert.Equal(TokenType.Plus, tokens[3].Type);
            Assert.Equal("+", tokens[3].Value);

            Assert.Equal(TokenType.Integer, tokens[4].Type);
            Assert.Equal("4", tokens[4].Value);

            Assert.Equal(TokenType.Semicolon, tokens[5].Type);
            Assert.Equal(";", tokens[5].Value);

            Assert.Equal(TokenType.EOF, tokens[6].Type);
        }
    }
}
