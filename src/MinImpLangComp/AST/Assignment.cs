namespace MinImpLangComp.AST
{
    public class Assignment : Statement
    {
        public string Identifier { get; }
        public Expression Expression { get; }

        public Assignment(string identifier, Expression expression)
        {
            Identifier = identifier;
            Expression = expression;
        }

    }
}
