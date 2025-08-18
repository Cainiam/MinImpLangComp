namespace MinImpLangComp.AST
{
    /// <summary>
    /// Statement that declares a constant with an optional static type annotation.
    /// </summary>
    public class ConstantDeclaration : Statement
    {
        /// <summary>
        /// Constant identifier.
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Expression providing the constant's value.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Optional type annotation (when provied by the source).
        /// </summary>
        public TypeAnnotation? TypeAnnotation { get; }

        /// <summary>
        /// Creates a <see cref="ConstantDeclaration"/>.
        /// </summary>
        /// <param name="identifier">Constant name.</param>
        /// <param name="expression">Initializer expression.</param>
        /// <param name="typeAnnotation">Optional type annotation.</param>
        public ConstantDeclaration(string identifier, Expression expression, TypeAnnotation? typeAnnotation)
        {
            Identifier = identifier;
            Expression = expression;
            TypeAnnotation = typeAnnotation;
        }
    }
}
