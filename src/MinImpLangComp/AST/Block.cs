using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
