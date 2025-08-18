namespace MinImpLangComp.AST
{
    /// <summary>
    /// Statement represting a <c>while</c> loop with a condition and a body.
    /// </summary>
    public class WhileStatement : Statement
    {
        /// <summary>
        /// Loop condition evaluated before each iteration.
        /// </summary>
        public Expression Condition { get; }

        /// <summary>
        /// Loop body executed while <see cref="Condition"/> is <c>true</c>.
        /// </summary>
        public Statement Body { get; }

        /// <summary>
        /// Creates a <see cref="WhileStatement"/>.
        /// </summary>
        /// <param name="condition">Condition expression.</param>
        /// <param name="body">Loop body.</param>
        public WhileStatement(Expression condition, Statement body)
        {
            Condition = condition;
            Body = body;
        }
    }
}
