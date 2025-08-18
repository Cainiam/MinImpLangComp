namespace MinImpLangComp.AST
{
    /// <summary>
    /// Statement that assigns the result of an expression to a variable.
    /// </summary>
    public class Assignment : Statement
    {
        /// <summary>
        /// Target variable name.
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Expression whose evaluated is assigned.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Creates an <see cref="Assignment"/> statement.
        /// </summary>
        /// <param name="identifier">Target variable name.</param>
        /// <param name="expression">Assigned expression.</param>
        public Assignment(string identifier, Expression expression)
        {
            Identifier = identifier;
            Expression = expression;
        }

    }
}
