namespace MinImpLangComp.AST
{
    /// <summary>
    /// Expression that constructs an array from a list of element expression.
    /// </summary>
    public class ArrayLiteral : Expression
    {
        /// <summary>
        /// Element expressions composing the array literal.
        /// </summary>
        public List<Expression> Elements { get; }

        /// <summary>
        /// Creates an <see cref="ArrayLiteral"/> expression.
        /// </summary>
        /// <param name="elements">Element expression.</param>
        public ArrayLiteral(List<Expression> elements)
        {
            Elements = elements;
        }
    }
}
