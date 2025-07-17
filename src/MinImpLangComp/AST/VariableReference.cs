using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinImpLangComp.AST
{
    public class VariableReference : Expression
    {
        public string Name { get; }

        public VariableReference(string name)
        {
            Name = name;
        }
    }
}
