namespace MinImpLangComp.AST
{
    /// <summary>
    /// Statement represting a C-style <c>for</c> loop.
    /// </summary>
    public class ForStatement : Statement
    {
        /// <summary>
        /// Initialization statement executed once before the loop body.
        /// </summary>
        public Statement Initializer { get; }

        /// <summary>
        /// Loop continuation condition evaluated before each iteration.
        /// </summary>
        public Expression Condition { get; }

        /// <summary>
        /// Increment statement executed after each iteration.
        /// </summary>
        public Statement Increment { get; }

        /// <summary>
        /// Loop body.
        /// </summary>
        public Statement Body { get; }

        /// <summary>
        /// Creates a <see cref="ForStatement"/>.
        /// </summary>
        /// <param name="initializer">Initialization statement.</param>
        /// <param name="condition">Loop condition.</param>
        /// <param name="increment">Increment statement.</param>
        /// <param name="body">Loop body.</param>
        public ForStatement(Statement initializer, Expression condition, Statement increment, Statement body)
        {
            Initializer = initializer;
            Condition = condition;
            Increment = increment;
            Body = body;
        }
    }
}
