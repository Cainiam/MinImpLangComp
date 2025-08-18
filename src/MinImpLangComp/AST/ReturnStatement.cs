namespace MinImpLangComp.AST
{
    /// <summary>
    /// Statement that returns a value from the current function.
    /// </summary>
    public class ReturnStatement : Statement
    {
        /// <summary>
        /// Expression yielding the return value.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Creates a <see cref="ReturnStatement"/>.
        /// </summary>
        /// <param name="expression">Expression to evaluate and return.</param>
        public ReturnStatement(Expression expression)
        {
            Expression = expression;
        }
    }
}
