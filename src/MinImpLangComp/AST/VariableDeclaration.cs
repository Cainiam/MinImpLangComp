namespace MinImpLangComp.AST
{
    public class VariableDeclaration : Statement
    {
        public string Identifier { get; }
        public Expression Expression { get; }
        public TypeAnnotation? TypeAnnotation { get; }

        public VariableDeclaration(string identifier, Expression expression, TypeAnnotation? typeAnnotation)
        {
            Identifier = identifier;
            Expression = expression;
            TypeAnnotation = typeAnnotation;
        }
    }
}

