namespace MinImpLangComp.AST
{
    /// <summary>
    /// Expression representing a boolean constant.
    /// </summary>
    public class BooleanLiteral : Expression
    {
        /// <summary>
        /// Boolean value of the literal.
        /// </summary>
        public bool Value { get; }

        /// <summary>
        /// Creates a <see cref="BooleanLiteral"/>.
        /// </summary>
        /// <param name="value">Boolean value.</param>
        public BooleanLiteral(bool value)
        {
            Value = value;
        }
    }
}
