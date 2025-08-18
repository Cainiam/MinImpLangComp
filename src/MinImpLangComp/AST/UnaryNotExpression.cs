namespace MinImpLangComp.AST
{
    /// <summary>
    /// Logical negation expression (i.e., <c>!operand</c>).
    /// </summary>
    public class UnaryNotExpression : Expression
    {
        /// <summary>
        /// Operand being negated.
        /// </summary>
        public Expression Operand { get; }

        /// <summary>
        /// Creates a <see cref="UnaryNotExpression"/>.
        /// </summary>
        /// <param name="operand">Expression to negate.</param>
        public UnaryNotExpression(Expression operand)
        {
            Operand = operand;
        }
    }
}
