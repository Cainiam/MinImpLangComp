namespace MinImpLangComp.AST
{
    public class UnaryNotExpression : Expression
    {
        public Expression Operand { get; }

        public UnaryNotExpression(Expression operand)
        {
            Operand = operand;
        }
    }
}
