namespace MinImpLangComp.AST
{
    /// <summary>
    /// Statement that consists solely of a single expression (evaluated for side effects or value).
    /// </summary>
    public class ExpressionStatement : Statement
    {
        /// <summary>
        /// The wrapped expression.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Creates an <see cref="ExpressionStatement"/>.
        /// </summary>
        /// <param name="expression">Expression to evaluate.</param>
        public ExpressionStatement(Expression expression)
        {
            Expression = expression;
        }
    }
}
