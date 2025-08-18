using MinImpLangComp.Lexing;
using Xunit;

namespace MinImpLangComp.Tests
{
    /// <summary>
    /// Unit tests for the <see cref="Lexer"/> covering literals, operators, keywords, string escapes, whitespace handling, and simple sequences.
    /// </summary>
    public class LexerTests
    {
        #region Helpers
        /// <summary>
        /// Lexes the entire input and returns all tokens up to (but no including) EOF.
        /// </summary>
        private static List<Token> LexAll(string input)
        {
            var lexer = new Lexer(input);
            var tokens = new List<Token>();
            Token t;
            while ((t = lexer.GetNextToken()).Type != TokenType.EOF) tokens.Add(t);
            return tokens;
        }
        #endregion

        /// <summary>
        /// Validates that single-token inputs are recognized with the correct type and value.
        /// </summary>
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

        /// <summary>
        /// Ensure EOF is returned after consuming the final token.
        /// </summary>
        [Fact]
        public void GetNextToken_ReturnsEOFAtEnd()
        {
            var lexer = new Lexer("10");
            lexer.GetNextToken();
            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.EOF, token.Type);
        }

        /// <summary>
        /// Unknown characters should produce an <see cref="TokenType.Unknow"/> token.
        /// </summary>
        [Fact]
        public void GetNextToken_ReturnsUnknow()
        {
            var lexer = new Lexer("@");
            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.Unknow, token.Type);
            Assert.Equal("@", token.Value);
        }

        /// <summary>
        /// Invalid numeric fortmats (multiple dots) should yield a lexical error.
        /// </summary>
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

        /// <summary>
        /// Leading/trailing whitespace is skipped before tokenizing.
        /// </summary>
        [Fact]
        public void GetNextToken_SkipsWhiteSpace()
        {
            var lexer = new Lexer("   50   ");
            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.Integer, token.Type);
            Assert.Equal("50", token.Value);
        }

        /// <summary>
        /// Tokenizes a short assignment statement into the expected sequenec.
        /// </summary>
        [Fact]
        public void GetNextToken_ReadsMultipleTokensInSequence()
        {
            var tokens = LexAll("var1 = 3 + 4;");

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
        }

        /// <summary>
        /// Keywords like set/if/else/while/for should be recognized.
        /// </summary>
        [Theory]
        [InlineData("set", TokenType.Set, "set")]
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

        /// <summary>
        /// String literal handles escaped quote characters.
        /// </summary>
        [Fact]
        public void GetNextToken_ReturnsStringLiteralWithEscapedQuote()
        {
            var lexer = new Lexer("\"hello \\\"world\\\"\"");
            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.StringLiteral, token.Type);
            Assert.Equal("hello \"world\"", token.Value);
        }

        /// <summary>
        /// String literal handles escaped newline.
        /// </summary>
        [Fact]
        public void GetNextToken_ReturnsStringLiteralWithNewline()
        {
            var lexer = new Lexer("\"line1\\nline2\"");
            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.StringLiteral, token.Type);
            Assert.Equal("line1\nline2", token.Value);
        }

        /// <summary>
        /// String literal handles escpaed backslashes.
        /// </summary>
        [Fact]
        public void GetNextToken_ReturnsStringLiteralWithBackSlash()
        {
            var lexer = new Lexer("\"C:\\\\Program Files\\\\\"");
            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.StringLiteral, token.Type);
            Assert.Equal("C:\\Program Files\\", token.Value);
        }

        /// <summary>
        /// Unterminated string should surface a lexical error with the original slice.
        /// </summary>
        [Fact]
        public void GetNextToken_UnterminatedString_ReturnsLexicalError()
        {
            var lexer = new Lexer("\"unclosed string");
            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.LexicalError, token.Type);
            Assert.Contains("unclosed string", token.Value);
            Assert.StartsWith("\"", token.Value);
        }

        /// <summary>
        /// Comparison and equality operators are recognized as multi-char tokens.
        /// </summary>
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

        /// <summary>
        /// Boolean keywords true/false are recognized.
        /// </summary>
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

        /// <summary>
        /// Logical-and / logical-or tokens.
        /// </summary>
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

        /// <summary>
        /// Prefix increment/decrement tokens.
        /// </summary>
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
        
        /// <summary>
        /// Basic string literal tokenization.
        /// </summary>
        [Fact]
        public void GetNextToken_ReturnsStringLiteralToken()
        {
            var lexer = new Lexer("\"hello world\"");
            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.StringLiteral, token.Type);
            Assert.Equal("hello world", token.Value);
        }

        /// <summary>
        /// Logical NOT tokenization.
        /// </summary>
        [Fact]
        public void GetNextToken_RetunsNotToken()
        {
            var lexer = new Lexer("!");
            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.Not, token.Type);
            Assert.Equal("!", token.Value);
        }

        /// <summary>
        /// Bitwise AND tokenization.
        /// </summary>
        [Fact]
        public void GetNextToken_ReturnsBitwiseAndToken()
        {
            var lexer = new Lexer("&");
            var token = lexer.GetNextToken();
            Assert.Equal(TokenType.BitwiseAnd, token.Type);
            Assert.Equal("&", token.Value);
        }

        /// <summary>
        /// Bitwise OR tokenization.
        /// </summary>
        [Fact]
        public void GetNextToken_ReturnsBitwiseOrToken()
        {
            var lexer = new Lexer("|");
            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.BitwiseOr, token.Type);
            Assert.Equal("|", token.Value);
        }

        /// <summary>
        /// Null keyword tokenization.
        /// </summary>
        [Fact]
        public void GetNextToken_ReturnsNullKeywordToken()
        {
            var lexer = new Lexer("null");
            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.Null, token.Type);
            Assert.Equal("null", token.Value);
        }

        /// <summary>
        /// Break token tokenization.
        /// </summary>
        [Fact]
        public void GetNextToken_ReturnsBreakToken()
        {
            var lexer = new Lexer("break");
            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.Break, token.Type);
            Assert.Equal("break", token.Value);
        }

        /// <summary>
        /// Continue keyword tokenization.
        /// </summary>
        [Fact]
        public void GetNextToken_ReturnsContinueToken()
        {
            var lexer = new Lexer("continue");
            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.Continue, token.Type);
            Assert.Equal("continue", token.Value);
        }

        /// <summary>
        /// Verifies <c>set</c> is recognized within a longer input.
        /// </summary>
        [Fact]
        public void Lexer_CanRecognizeSetKeyword()
        {
            var tokens = LexAll("set x = 10;");

            Assert.Contains(tokens, t => t.Type == TokenType.Set);
        }

        /// <summary>
        /// Bind keyword tokenization.
        /// </summary>
        [Fact]
        public void Lexer_CanRecognizeBindKeyword()
        {
            var lexer = new Lexer("bind");
            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.Bind, token.Type);
            Assert.Equal("bind", token.Value);
        }

        /// <summary>
        /// Type keywords tokenization (int/float/bool/string).
        /// </summary>
        [Theory]
        [InlineData("int", TokenType.TypeInt, "int")]
        [InlineData("float", TokenType.TypeFloat, "float")]
        [InlineData("bool", TokenType.TypeBool, "bool")]
        [InlineData("string", TokenType.TypeString, "string")]
        public void GetNextToken_ReturnsTypeKeywords(string input, TokenType expectedType, string expectedValue)
        {
            var lexer = new Lexer(input);
            var token = lexer.GetNextToken();

            Assert.Equal(expectedType, token.Type);
            Assert.Equal(expectedValue, token.Value);
        }

        /// <summary>
        /// Verifies a typed declaration breaks into the expected token sequence.
        /// </summary>
        [Fact]
        public void GetNextToken_ReturnsTokensForTypedDeclaration()
        {
            var tokens = LexAll("set x: int = 5;");

            Assert.Collection(tokens,
                t => Assert.Equal(TokenType.Set, t.Type),
                t =>
                {
                    Assert.Equal(TokenType.Identifier, t.Type);
                    Assert.Equal("x", t.Value);
                },
                t => Assert.Equal(TokenType.Colon, t.Type),
                t =>
                {
                    Assert.Equal(TokenType.TypeInt, t.Type);
                    Assert.Equal("int", t.Value);
                },
                t => Assert.Equal(TokenType.Assign, t.Type),
                t =>
                {
                    Assert.Equal(TokenType.Integer, t.Type);
                    Assert.Equal("5", t.Value);
                },
                t => Assert.Equal(TokenType.Semicolon, t.Type)
            );
        }

        /// <summary>
        /// Colon tokenization.
        /// </summary>
        [Fact]
        public void GetNextToken_ReturnsColonToken()
        {
            var lexer = new Lexer(":");
            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.Colon, token.Type);
            Assert.Equal(":", token.Value);
        }
    }
}
