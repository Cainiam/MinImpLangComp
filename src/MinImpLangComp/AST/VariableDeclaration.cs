namespace MinImpLangComp.AST
{
    /// <summary>
    /// Statement that declares a variable with an initializer and an optional type annotatioN.
    /// </summary>
    public class VariableDeclaration : Statement
    {
        /// <summary>
        /// Variable name.
        /// </summary>
        public string Identifier { get; }
        /// <summary>
        /// Initializer expression whose value is assigned to the variable.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Optional static type annotation as written in the source.
        /// </summary>
        public TypeAnnotation? TypeAnnotation { get; }

        /// <summary>
        /// Creates a <see cref="VariableDeclaration"/>.
        /// </summary>
        /// <param name="identifier">Variable name.</param>
        /// <param name="expression">Initializer expression.</param>
        /// <param name="typeAnnotation">Optional type annotation.</param>
        public VariableDeclaration(string identifier, Expression expression, TypeAnnotation? typeAnnotation)
        {
            Identifier = identifier;
            Expression = expression;
            TypeAnnotation = typeAnnotation;
        }
    }
}

