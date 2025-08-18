namespace MinImpLangComp.AST
{
    /// <summary>
    /// Expression that reads the value of a variable by name.
    /// </summary>
    public class VariableReference : Expression
    {
        /// <summary>
        /// Variable name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Creates a <see cref="VariableReference"/>.
        /// </summary>
        /// <param name="name">Variable name.</param>
        public VariableReference(string name)
        {
            Name = name;
        }
    }
}
