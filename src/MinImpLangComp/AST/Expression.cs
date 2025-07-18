namespace MinImpLangComp.AST
{
    public abstract class Expression : Node
    {
    }

    public class IntegerLiteral : Expression
    {
        public int Value { get; }

        public IntegerLiteral(int value)
        {
            Value = value;
        }
    }

    public class FloatLiteral : Expression
    {
        public double Value { get; }

        public FloatLiteral(double value)
        {
            Value = value;
        }
    }

    public class BinaryExpression : Expression
    {
        public Expression Left { get; }
        public Expression Right { get; }
        public string Operator { get; }

        public BinaryExpression(Expression left, string oper , Expression right)
        {
            Left = left;
            Right = right;
            Operator = oper;
        }
    }
}
