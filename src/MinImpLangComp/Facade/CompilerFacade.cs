using System.Text;
using MinImpLangComp.AST;
using MinImpLangComp.Lexing;
using MinImpLangComp.Parsing;
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
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var parseProgram = parser.GetType().GetMethod("ParseProgram", Type.EmptyTypes);
            if (parseProgram != null)
            {
                var result = parseProgram.Invoke(parser, null) as List<Statement>;
                return result ?? new List<Statement>();
            }

            var statements = new List<Statement>();
            while(!IsParserAtEnd(parser))
            {
                var stMethod = parser.GetType().GetMethod("ParseStatement");
                if (stMethod == null) throw new NotImplementedException("Parser.ParseStatement() is missing");
                var st = stMethod.Invoke(parser, null) as Statement;
                if (st != null) statements.Add(st);
            }
            return statements;
        }

        private static bool IsParserAtEnd(object parser)
        {
            var prop = parser.GetType().GetProperty("IsAtEnd");
            if (prop != null && prop.PropertyType == typeof(bool)) return (bool)prop.GetValue(parser);
            return false;
        }
    }
}
