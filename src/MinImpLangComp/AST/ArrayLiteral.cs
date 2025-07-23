namespace MinImpLangComp.AST
{
    public class ArrayLiteral : Expression
    {
        public List<Expression> Elements { get; }

        public ArrayLiteral(List<Expression> elements)
        {
            Elements = elements;
        }
    }
}
