namespace MinImpLangComp.Lexing
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
        Set,
        Bind,
        If,
        Else,
        While,
        For,
        StringLiteral,
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
        Modulo,
        Not,
        BitwiseAnd,
        BitwiseOr,
        LeftBracket,
        RightBracket,
        Null,
        Break,
        Continue,
        TypeInt,
        TypeFloat,
        TypeBool,
        TypeString,
        Colon
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
