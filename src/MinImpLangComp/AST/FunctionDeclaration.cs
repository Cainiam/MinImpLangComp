namespace MinImpLangComp.AST
{
    /// <summary>
    /// Statement that declares a named function with parameters and a body.
    /// </summary>
    public class FunctionDeclaration : Statement
    {
        /// <summary>
        /// Function name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Parameter names in declaration order.
        /// </summary>
        public List<string> Parameters { get; }
        
        /// <summary>
        /// Function body.
        /// </summary>
        public Block Body { get; }

        /// <summary>
        /// Creates a <see cref="FunctionDeclaration"/>.
        /// </summary>
        /// <param name="name">Function name.</param>
        /// <param name="parameters">Paramter names.</param>
        /// <param name="body">Function body.</param>
        public FunctionDeclaration(string name, List<string> parameters, Block body)
        {
            Name = name;
            Parameters = parameters; 
            Body = body;
        }
    }
}
