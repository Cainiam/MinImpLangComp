namespace MinImpLangComp.AST
{
    /// <summary>
    /// Expression that reads an element from an array variable using an index expression.
    /// </summary>
    public class ArrayAccess : Expression
    {
        /// <summary>
        /// Name of the array variable.
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Index expression used to access the array element.
        /// </summary>
        public Expression Index { get; }

        /// <summary>
        /// Creates an <see cref="ArrayAccess"/> expression.
        /// </summary>
        /// <param name="identifier">Array variable name.</param>
        /// <param name="index">Index expression.</param>
        public ArrayAccess(string identifier, Expression index)
        {
            Identifier = identifier;
            Index = index;
        }
    }
}
