namespace MinImpLangComp.AST
{
    public class FunctionCall : Expression
    {
        public string Name { get; }
        public List<Expression> Arguments { get; }

        public FunctionCall(string name, List<Expression> arguments)
        {
            Name = name;
            Arguments = arguments;
        }
    }
}
