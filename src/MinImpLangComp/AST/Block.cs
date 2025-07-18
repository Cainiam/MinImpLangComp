namespace MinImpLangComp.AST
{
    public class Block : Statement
    {
        public List<Statement> Statements {  get; }

        public Block(List<Statement> statements) 
        {
            Statements = statements;
        }
    }
}
