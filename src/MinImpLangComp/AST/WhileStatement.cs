namespace MinImpLangComp.AST
{
    public class WhileStatement : Statement
    {
        public Expression Condition { get; }
        public Statement Body { get; }

        public WhileStatement(Expression condition, Statement body)
        {
            Condition = condition;
            Body = body;
        }
    }
}
