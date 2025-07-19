using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinImpLangComp.AST
{
    public class ForStatement : Statement
    {
        public string Variable { get; }
        public Expression Start { get; }
        public Expression End { get; }
        public Statement Body { get; }

        public ForStatement(string variable, Expression start, Expression end, Statement body)
        {
            Variable = variable;
            Start = start;
            End = end;
            Body = body;
        }
    }
}
