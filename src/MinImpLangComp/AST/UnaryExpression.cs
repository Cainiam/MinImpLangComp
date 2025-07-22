namespace MinImpLangComp.AST
{
    public class UnaryExpression : Expression
    {
        public OperatorType Operator { get; }
        public string Identifier { get; }

        public UnaryExpression(OperatorType oper, string identifier)
        {
            Operator = oper;
            Identifier = identifier;
        }
    }
}
