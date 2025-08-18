namespace MinImpLangComp.AST
{
    /// <summary>
    /// Supported operator kinds for binary and unary operations.
    /// </summary>
    public enum OperatorType
    {
        /// <summary>
        /// Arithmetic addition or string concatenation.
        /// </summary>
        Plus,
        /// <summary>
        /// Arithmetic substraction.
        /// </summary>
        Minus,
        /// <summary>
        /// Arithmetic multiplication.
        /// </summary>
        Multiply,
        /// <summary>
        /// Arithmetic division.
        /// </summary>
        Divide,
        /// <summary>
        /// Arithmetic modulo (remainder).
        /// </summary>
        Modulo,
        /// <summary>
        /// Unary post/prefix increment (domain-specific usage).
        /// </summary>
        PlusPlus,
        /// <summary>
        /// Unary post/prefix decrement (domain-specific usage).
        /// </summary>
        MinusMinus,
        /// <summary>
        /// Relational: less-than.
        /// </summary>
        Less,
        /// <summary>
        /// Relational: greater-than.
        /// </summary>
        Greater,
        /// <summary>
        /// Relational: less-than-or-equal.
        /// </summary>
        LessEqual,
        /// <summary>
        /// Relational: greater-than-or-equal.
        /// </summary>
        GreaterEqual,
        /// <summary>
        /// Relational: equality.
        /// </summary>
        Equalequal,
        /// <summary>
        /// Relational: inequality.
        /// </summary>
        NotEqual,
        /// <summary>
        /// Logical AND with short-circuit.
        /// </summary>
        AndAnd,
        /// <summary>
        /// Logicial OR with short-ciruit.
        /// </summary>
        OrOr,
        /// <summary>
        /// Bitwise AND.
        /// </summary>
        BitwiseAnd,
        /// <summary>
        /// Bitwise OR.
        /// </summary>
        BitwiseOr
    }
}
