namespace MinImpLangComp.AST
{
    /// <summary>
    /// Statement representing a conditional branch with an optional else branch.
    /// </summary>
    public class IfStatement : Statement
    {
        /// <summary>
        /// Condition to evalute.
        /// </summary>
        public Expression Condition { get; }

        /// <summary>
        /// Statement executed when <see cref="Condition"/> is <c>true</c>.
        /// </summary>
        public Statement ThenBranch { get; }

        /// <summary>
        /// Optional statement executed when <see cref="Condition"/> is <c>false</c>.
        /// </summary>
        public Statement? ElseBranch { get; }

        /// <summary>
        /// Creates an <see cref="IfStatement"/>.
        /// </summary>
        /// <param name="condition">Condition expression.</param>
        /// <param name="thenBranch">Branch executed when condition is true.</param>
        /// <param name="elseBranch">Optional else branch.</param>
        public IfStatement(Expression condition, Statement thenBranch, Statement? elseBranch = null)
        {
            Condition = condition;
            ThenBranch = thenBranch;
            ElseBranch = elseBranch;
        }
    }
}
