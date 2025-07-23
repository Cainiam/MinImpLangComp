namespace MinImpLangComp.AST
{
    public class ArrayAssignment : Statement
    {
        public string Identifier { get; }
        public Expression Index { get; }
        public Expression Value { get; }

        public ArrayAssignment(string identifier, Expression index, Expression value)
        {
            Identifier = identifier;
            Index = index;
            Value = value;
        }
    }
}
