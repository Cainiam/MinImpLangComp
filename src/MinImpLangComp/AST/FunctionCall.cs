namespace MinImpLangComp.AST
{
    /// <summary>
    /// Expression that invokes a function by name with arguments.
    /// </summary>
    public class FunctionCall : Expression
    {
        /// <summary>
        /// Callee name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Actual argument in call order.
        /// </summary>
        public List<Expression> Arguments { get; }

        /// <summary>
        /// Creates a <see cref="FunctionCall"/>.
        /// </summary>
        /// <param name="name">Function name.</param>
        /// <param name="arguments">Argument expression.</param>
        public FunctionCall(string name, List<Expression> arguments)
        {
            Name = name;
            Arguments = arguments;
        }
    }
}
