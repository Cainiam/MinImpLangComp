namespace MinImpLangComp.AST
{
    public class FunctionDeclaration : Statement
    {
        public string Name { get; }
        public List<string> Parameters { get; }
        public Block Body { get; }

        public FunctionDeclaration(string name, List<string> parameters, Block body)
        {
            Name = name;
            Parameters = parameters; 
            Body = body;
        }
    }
}
