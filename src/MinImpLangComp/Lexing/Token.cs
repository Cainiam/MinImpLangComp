namespace MinImpLangComp.Lexing
{ 
    /// <summary>
    /// Token kinds produced by the lexer.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Unknow"/> is (un)intentionnaly spelled this way to preserve backward compatibility with existing code/test.
    /// </para>
    /// </remarks>
    public enum TokenType
    {
        Integer,
        Float,
        Identifier,
        StringLiteral,
        Set,
        Bind,
        Plus,
        Minus,
        Multiply,
        Divide,
        Modulo,
        Assign,
        Semicolon,
        Colon,
        Dot,
        Comma,
        LeftParen,
        RightParen,
        LeftBrace,
        RightBrace,
        LeftBracket,
        RightBracket,
        If,
        Else,
        While,
        For,
        Null,
        Break,
        Continue,
        Less,
        Greater,
        LessEqual,
        GreaterEqual,
        Equalequal,
        NotEqual,
        True,
        False,
        AndAnd,
        OrOr,
        PlusPlus,
        MinusMinus,
        Function,
        Return,
        Not,
        BitwiseAnd,
        BitwiseOr,
        TypeInt,
        TypeFloat,
        TypeBool,
        TypeString,
        EOF,
        LexicalError,
        Unknow // Unknonw but kept as-is for compatibility
    }

    /// <summary>
    /// A single lexical token: its <see cref="Type"/> and the original <see cref="Value"/> lexeme.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// The token kind.
        /// </summary>
        public TokenType Type { get; }

        /// <summary>
        /// The original lexeme for this token.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Creates a new <see cref="Token"/>.
        /// </summary>
        /// <param name="type">Token type.</param>
        /// <param name="value">Lexeme text associated with this token.</param>
        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        /// <inheritdoc/>
        public override string ToString() => $"{Type}: {Value}";
    }
}
