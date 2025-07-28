namespace MinImpLangComp.AST
{
    public class ConstantDeclaration : Statement
    {
        public string Identifier { get; }
        public Expression Expression { get; }

        public ConstantDeclaration(string identifier, Expression expression)
        {
            Identifier = identifier;
            Expression = expression;
        }
    }
}
