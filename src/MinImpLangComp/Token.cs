namespace MinImpLangComp
{ 
    public enum TokenType
    {
        Integer,
        Float,
        Identifier,
        Plus,
        Minus,
        Multiply,
        Divide,
        Assign,
        Semicolon,
        Dot,
        Comma,
        LeftParen,
        RightParen,
        LeftBrace,
        RightBrace,
        EOF,
        LexicalError,
        Unknow,
        Let,
        If,
        Else,
        While,
        For,
        StringLiteral
    }

    public class Token
    {
        public TokenType Type { get; }
        public string Value { get; }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Type}: {Value}";
        }
    }
}
