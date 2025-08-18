namespace MinImpLangComp.AST
{
    /// <summary>
    /// Statement that assigns a value to an element of an array variable.
    /// </summary>
    public class ArrayAssignment : Statement
    {
        /// <summary>
        /// Name of the target array variable.
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Index expression where the assignment occurs.
        /// </summary>
        public Expression Index { get; }

        /// <summary>
        /// Exoression providing the value to assign
        /// </summary>
        public Expression Value { get; }

        /// <summary>
        /// Crates an <see cref="ArrayAssignment"/> statement.
        /// </summary>
        /// <param name="identifier">Array variable name.</param>
        /// <param name="index">Index expression.</param>
        /// <param name="value">Assigned value expression.</param>
        public ArrayAssignment(string identifier, Expression index, Expression value)
        {
            Identifier = identifier;
            Index = index;
            Value = value;
        }
    }
}
