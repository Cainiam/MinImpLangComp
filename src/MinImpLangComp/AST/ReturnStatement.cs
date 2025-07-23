namespace MinImpLangComp.AST
{
    public class ReturnStatement : Statement
    {
        public Expression Expression { get; }

        public ReturnStatement(Expression expression)
        {
            Expression = expression;
        }
    }
}
