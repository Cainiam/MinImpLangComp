namespace MinImpLangComp.AST
{
    /// <summary>
    /// Statement representing a block (sequence) of statements.
    /// </summary>
    public class Block : Statement
    {
        /// <summary>
        /// Statements contained in the block, in order.
        /// </summary>
        public List<Statement> Statements {  get; }

        /// <summary>
        /// Creates a <see cref="Block"/> statement.
        /// </summary>
        /// <param name="statements">Statements to execute squentially.</param>
        public Block(List<Statement> statements) 
        {
            Statements = statements;
        }
    }
}
