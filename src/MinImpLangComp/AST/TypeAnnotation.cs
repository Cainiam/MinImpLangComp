using MinImpLangComp.Lexing;

namespace MinImpLangComp.AST
{
    /// <summary>
    /// Represents a (possibily optional) static type annotation from the source code.
    /// </summary>
    public class TypeAnnotation
    {
        /// <summary>
        /// Human-readable type name as written in source (e.g., "int", "float" (double precision), "bool", "string").
        /// </summary>
        public string TypeName { get; }

        /// <summary>
        /// Token kind corresponding to the type keyword.
        /// </summary>
        public TokenType TypeToken { get; }

        /// <summary>
        /// Creates a <see cref="TypeAnnotation"/>.
        /// </summary>
        /// <param name="typeName">Type name as written in source.</param>
        /// <param name="typeToken">Token kind for the type keyword.</param>
        public TypeAnnotation(string typeName, TokenType typeToken)
        {
            TypeName = typeName;
            TypeToken = typeToken;
        }

        /// <summary>
        /// Returns the source-facing type name.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return TypeName;
        }
    }
}
