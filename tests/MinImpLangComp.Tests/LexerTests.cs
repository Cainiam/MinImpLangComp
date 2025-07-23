using MinImpLangComp.Lexing;
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
        public void GetNextToken_ReturnsExpectedToken(string input, TokenType expectedType, string expectedValue)
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

        [Theory]
        [InlineData("let", TokenType.Let, "let")]
        [InlineData("if", TokenType.If, "if")]
        [InlineData("else", TokenType.Else, "else")]
        [InlineData("while", TokenType.While, "while")]
        [InlineData("for", TokenType.For, "for")]
        public void GetNextToken_ReturnsKeywordTokens(string input, TokenType expectedType, string expectedValue)
        {
            var lexer = new Lexer(input);
            var token = lexer.GetNextToken();

            Assert.Equal(expectedType, token.Type);
            Assert.Equal(expectedValue, token.Value);
        }

        [Fact]
        public void GetNextToken_ReturnsStringLiteralWithEscapedQuote()
        {
            var lexer = new Lexer("\"hello \\\"world\\\"\"");
            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.StringLiteral, token.Type);
            Assert.Equal("hello \"world\"", token.Value);
        }

        [Fact]
        public void GetNextToken_ReturnsStringLiteralWithNewline()
        {
            var lexer = new Lexer("\"line1\\nline2\"");
            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.StringLiteral, token.Type);
            Assert.Equal("line1\nline2", token.Value);
        }

        [Fact]
        public void GetNextToken_ReturnsStringLiteralWithBackSlash()
        {
            var lexer = new Lexer("\"C:\\\\Program Files\\\\\"");
            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.StringLiteral, token.Type);
            Assert.Equal("C:\\Program Files\\", token.Value);
        }

        [Fact]
        public void GetNextToken_UnterminatedString_ReturnsLexicalError()
        {
            var lexer = new Lexer("\"unclosed string");
            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.LexicalError, token.Type);
            Assert.Contains("unclosed string", token.Value);
            Assert.StartsWith("\"", token.Value);
        }

        [Theory]
        [InlineData("<=", TokenType.LessEqual, "<=")]
        [InlineData(">=", TokenType.GreaterEqual, ">=")]
        [InlineData("==", TokenType.Equalequal, "==")]
        [InlineData("!=", TokenType.NotEqual, "!=")]
        public void GetNextToken_ReturnsComparisonAndEqualityOperators(string input, TokenType expectedType, string expectedValue)
        {
            var lexer = new Lexer(input);
            var token = lexer.GetNextToken();

            Assert.Equal(expectedType, token.Type);
            Assert.Equal(expectedValue, token.Value);
        }

        [Theory]
        [InlineData("true", TokenType.True, "true")]
        [InlineData("false", TokenType.False, "false")]
        public void GetNextToken_ReturnsTrueAndFalseTokens(string input, TokenType expectedType, string excpectedValue)
        {
            var lexer = new Lexer(input);
            var token = lexer.GetNextToken();

            Assert.Equal(expectedType, token.Type);
            Assert.Equal(excpectedValue, token.Value);
        }

        [Fact]
        public void GetNextToken_ReturnsAndAndOrOrTokens()
        {
            var lexer = new Lexer("&& ||");

            var token1 = lexer.GetNextToken();
            Assert.Equal(TokenType.AndAnd, token1.Type);
            Assert.Equal("&&", token1.Value);
            var token2 = lexer.GetNextToken();
            Assert.Equal(TokenType.OrOr, token2.Type);
            Assert.Equal("||", token2.Value);
        }
        
        [Fact]
        public void GetNextToken_ReturnsPlusPlusAndMinusMinusTokens()
        {
            var lexer = new Lexer("++ --");

            var token1 = lexer.GetNextToken();
            Assert.Equal(TokenType.PlusPlus, token1.Type);
            Assert.Equal("++", token1.Value);
            var token2 = lexer.GetNextToken();
            Assert.Equal(TokenType.MinusMinus, token2.Type);
            Assert.Equal("--", token2.Value);
        }

        [Fact]
        public void GetNextToken_ReturnsStringLiteralToken()
        {
            var lexer = new Lexer("\"hello world\"");
            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.StringLiteral, token.Type);
            Assert.Equal("hello world", token.Value);
        }

        [Fact]
        public void GetNextToken_RetunsNotToken()
        {
            var lexer = new Lexer("!");
            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.Not, token.Type);
            Assert.Equal("!", token.Value);
        }

        [Fact]
        public void GetNextToken_ReturnsBitwiseAndToken()
        {
            var lexer = new Lexer("&");
            var token = lexer.GetNextToken();
            Assert.Equal(TokenType.BitwiseAnd, token.Type);
            Assert.Equal("&", token.Value);
        }

        [Fact]
        public void GetNextToken_ReturnsBitwiseOrToken()
        {
            var lexer = new Lexer("|");
            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.BitwiseOr, token.Type);
            Assert.Equal("|", token.Value);
        }
    }
}
