namespace MinImpLangComp.AST
{
    public class VariableReference : Expression
    {
        public string Name { get; }

        public VariableReference(string name)
        {
            Name = name;
        }
    }
}
