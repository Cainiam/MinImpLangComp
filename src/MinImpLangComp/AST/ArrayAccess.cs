namespace MinImpLangComp.AST
{
    public class ArrayAccess : Expression
    {
        public string Identifier { get; }
        public Expression Index { get; }

        public ArrayAccess(string identifier, Expression index)
        {
            Identifier = identifier;
            Index = index;
        }
    }
}
