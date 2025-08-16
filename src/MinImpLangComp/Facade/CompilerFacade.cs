using System.Text;
using MinImpLangComp.AST;
using MinImpLangComp.ILGeneration;

namespace MinImpLangComp.Facade
{
    public static class CompilerFacade
    {
        public static string Run(string source, string? input = null)
        {
            var statements = ParseToStatement(source);
            ILGeneratorUtils.BuildAndRegisterFunctions(statements);
            var originalOut = Console.Out;
            var origianlIn = Console.In;
            using var writer = new StringWriter(new StringBuilder());
            try
            {
                Console.SetOut(writer);
                if (input != null) Console.SetIn(new StringReader(input));
                ILGeneratorRunner.GenerateAndRunIL(statements);
                writer.Flush();
                return writer.ToString();
            }
            finally
            {
                Console.SetOut(originalOut);
                Console.SetIn(origianlIn);
            }
        }

        private static List<Statement> ParseToStatement(string source)
        {
            // ToDo : Le parser ici

            throw new NotImplementedException("Parser not pluged in");
        }
    }
}
