namespace MinImpLangComp.AST
{
    /// <summary>
    /// Base type for all expression.
    /// </summary>
    public abstract class Expression : Node
    {
    }

    /// <summary>
    /// Integer numeric literal expression.
    /// </summary>
    public class IntegerLiteral : Expression
    {
        /// <summary>
        /// Integer value.
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Creates an <see cref="IntegerLiteral"/>.
        /// </summary>
        /// <param name="value">Integer value.</param>
        public IntegerLiteral(int value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// Floating-point numeric literal expression (double precision).
    /// </summary>
    public class FloatLiteral : Expression
    {
        /// <summary>
        /// Double value.
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// Creates a <see cref="FloatLiteral"/>
        /// </summary>
        /// <param name="value">Double value.</param>
        public FloatLiteral(double value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// Binary operator expression (e. g., +, -, *, /, %, comparisons, logical ops).
    /// </summary>
    public class BinaryExpression : Expression
    {
        /// <summary>
        /// Left-hand operand.
        /// </summary>
        public Expression Left { get; }

        /// <summary>
        /// Right-hand operand.
        /// </summary>
        public Expression Right { get; }

        /// <summary>
        /// Operator applied to <see cref="Left"/> and <see cref="Right"/>.
        /// </summary>
        public OperatorType Operator { get; }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/>.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="oper">Operator.</param>
        /// <param name="right">Right operand.</param>
        public BinaryExpression(Expression left, OperatorType oper , Expression right)
        {
            Left = left;
            Right = right;
            Operator = oper;
        }
    }
}
