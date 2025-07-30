namespace MinImpLangComp.AST
{
    public class ConstantDeclaration : Statement
    {
        public string Identifier { get; }
        public Expression Expression { get; }
        public TypeAnnotation? TypeAnnotation { get; }

        public ConstantDeclaration(string identifier, Expression expression, TypeAnnotation? typeAnnotation)
        {
            Identifier = identifier;
            Expression = expression;
            TypeAnnotation = typeAnnotation;
        }
    }
}
