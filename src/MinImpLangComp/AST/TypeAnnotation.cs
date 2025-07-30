using MinImpLangComp.Lexing;

namespace MinImpLangComp.AST
{
    public class TypeAnnotation
    {
        public string TypeName { get; }
        public TokenType TypeToken { get; }

        public TypeAnnotation(string typeName, TokenType typeToken)
        {
            TypeName = typeName;
            TypeToken = typeToken;
        }

        public override string ToString()
        {
            return TypeName;
        }
    }
}
