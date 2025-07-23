namespace MinImpLangComp.AST
{
    public class StringLiteral : Expression
    {
        public string Value { get; }
        
        public StringLiteral(string value)
        {
            Value = value;
        }
    }
}
