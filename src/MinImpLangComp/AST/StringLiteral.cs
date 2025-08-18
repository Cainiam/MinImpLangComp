namespace MinImpLangComp.AST
{
    /// <summary>
    /// Expression representing a string constant.
    /// </summary>
    public class StringLiteral : Expression
    {
        /// <summary>
        /// String value of the literal.
        /// </summary>
        public string Value { get; }
        
        /// <summary>
        /// Creates a <see cref="StringLiteral"/>.
        /// </summary>
        /// <param name="value">String value.</param>
        public StringLiteral(string value)
        {
            Value = value;
        }
    }
}
