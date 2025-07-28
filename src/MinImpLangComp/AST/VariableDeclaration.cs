namespace MinImpLangComp.AST
{
    public class VariableDeclaration : Statement
    {
        public string Identifier { get; }
        public Expression Expression { get; }

        public VariableDeclaration(string identifier, Expression expression)
        {
            Identifier = identifier;
            Expression = expression;
        }
    }
}

