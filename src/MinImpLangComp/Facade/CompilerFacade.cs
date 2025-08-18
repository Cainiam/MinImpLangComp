using System;
using MinImpLangComp.Runtime;
using MinImpLangComp.AST;
using MinImpLangComp.Lexing;
using MinImpLangComp.Parsing;
using MinImpLangComp.ILGeneration;

namespace MinImpLangComp.Facade
{
    /// <summary>
    /// High-level facade that compiles and runs MinImp source via IL generation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Responsibilities:
    /// <list type="bullet">
    /// <item><description>Tokenize &amp; parse source into AST statements.</description></item>
    /// <item><description>Build and register dynamic functions for calls within the script.</description></item>
    /// <item><description>Manage runtime I/O buffer (<see cref="RuntimeIO"/>) and optional stdin redirection.</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Thread-safety: execution is seralized via an internal lock to protect shared state such as <see cref="Console.In"/> and dynamic function registry.
    /// </para>
    /// </remarks>
    public static class CompilerFacade
    {
        private static readonly object _runLock = new object();

        /// <summary>
        /// Compiles and executes the given MinImp <paramref name="source"/> and returns the captured stdout.
        /// </summary>
        /// <param name="source">MinImp source code.</param>
        /// <param name="input">Optional redirected stdin for the program (passed to <see cref="Console.SetIn(System.IO.TextReader)"/>.</param>
        /// <returns>Captured output produced via <see cref="RuntimeIO.Print(object?)"/>.</returns>
        public static string Run(string source, string? input = null)
        {
            lock (_runLock)
            { 
                var statements = ParseToStatement(source);
                ILGeneratorUtils.BuildAndRegisterFunctions(statements);
                RuntimeIO.Clear();
                var originalIn = Console.In;
                try
                {
                    if (input != null) Console.SetIn(new StringReader(input));
                    ILGeneratorRunner.RunScript(statements);
                    var output = RuntimeIO.Consume();
                    return output;
                }
                finally
                {
                    Console.SetIn(originalIn);
                    ILGeneratorUtils.ClearFunctionRegistry();
                }
            }
        }

        /// <summary>
        /// Parses the source into a list of statements. Uses the parser's <c>ParseProgram</c> when available, otherwwise falls back to repeatedly calling <c>ParseStatement</c> until <c>IsAtEnd</c>.
        /// </summary>
        /// <param name="source">Source statement to be parsed.</param>
        /// <returns><see cref="List{T}"/> of parsed <see cref="Statement"/>.</returns>
        /// <exception cref="NotImplementedException">Only throws if ParseStatement() is not implemented in Parser.</exception>
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

        /// <summary>
        /// Reflectively checks the parser's <c>IsAtEnd</c> boolean property.
        /// </summary>
        /// <param name="parser">Current parser to be checked.</param>
        /// <returns><c>True</c> if parsed is at end, if not <c>false</c>.</returns>
        private static bool IsParserAtEnd(object parser)
        {
            var prop = parser.GetType().GetProperty("IsAtEnd");
            if (prop != null && prop.PropertyType == typeof(bool)) return (bool)prop.GetValue(parser);
            return false;
        }
    }
}
