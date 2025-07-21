namespace MinImpLangComp.AST
{
    public class ForStatement : Statement
    {
        public Statement Initializer { get; }
        public Expression Condition { get; }
        public Statement Increment { get; }
        public Statement Body { get; }

        public ForStatement(Statement initializer, Expression condition, Statement increment, Statement body)
        {
            Initializer = initializer;
            Condition = condition;
            Increment = increment;
            Body = body;
        }
    }
}
