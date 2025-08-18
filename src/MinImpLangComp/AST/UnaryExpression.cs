namespace MinImpLangComp.AST
{
    /// <summary>
    /// Unary operation that targets a variable identifier (e.g., ++x or --x).
    /// </summary>
    public class UnaryExpression : Expression
    {
        /// <summary>
        /// Unary operator (<see cref="OperatorType.PlusPlus"/> or <see cref="OperatorType.MinusMinus"/>).
        /// </summary>
        public OperatorType Operator { get; }

        /// <summary>
        /// Target variable name.
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Creates a <see cref="UnaryExpression"/>.
        /// </summary>
        /// <param name="oper">Unary operator.</param>
        /// <param name="identifier">Target variable.</param>
        public UnaryExpression(OperatorType oper, string identifier)
        {
            Operator = oper;
            Identifier = identifier;
        }
    }
}
