namespace MinImpLangComp.AST
{
    public class BooleanLiteral : Expression
    {
        public bool Value { get; }

        public BooleanLiteral(bool value)
        {
            Value = value;
        }
    }
}
