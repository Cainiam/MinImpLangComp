using System;
using MinImpLangComp.Lexing;
using MinImpLangComp.Parsing;
using MinImpLangComp.Exceptions;
using MinImpLangComp.Interpreting;
using MinImpLangComp.AST;

namespace MinImpLangComp.ReplLoop
{
    /// <summary>
    /// Simple intactive Read-Eval-Print Loop (REPL) for MinImpLangComp.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The REPL keeps a single <see cref="Interpreter"/> instance across inputs, so variables, functions and constant defined by user persist during the session.
    /// </para>
    /// </remarks>
    public static class Repl
    {

        /// <summary>
        /// Runs the interactive loop untiler user types <c>exit</c>.
        /// </summary>
        public static void Run()
        {
            var interpreter = new Interpreter();
            WriteInfoLine("Bienvenue sur le REPL de MinImpLangComp. Ecrivez 'exit' pour quitter celui-ci lorsque vous avez terminé.");
            while(true)
            {
                Console.WriteLine();
                Console.Write("~ ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) continue;
                if (input.Trim().ToLower() == "exit") break;
                try
                {
                    var result = EvaluateInput(input, interpreter);
                    Console.ForegroundColor = ConsoleColor.Blue;
                    WriteInfoLine(" " + result);
                    Console.ResetColor();
                }
                catch (ParsingException pex)
                {
                    WriteErrorLine($"/!\\ Syntax Error ~ {pex.Message}");
                }
                catch (RuntimeException rex)
                {
                    WriteErrorLine($"/!\\ Runtime Error ~ {rex.Message}");
                }
                catch (Exception ex)
                {
                    WriteErrorLine($"/!\\ Unknow Error ~ {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Tokenizes, parses and evaluates a single line of input using the provided interpreter.
        /// </summary>
        /// <param name="input">User input to evaluate.</param>
        /// <param name="interpreter">Shared interpreter instance.</param>
        /// <returns>The computed result (may be null).</returns>
        private static object EvaluateInput(string input, Interpreter interpreter)
        {
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var statement = parser.ParseStatement();

            if (statement is ExpressionStatement expressionStatement && expressionStatement.Expression is BinaryExpression binaryExpression 
                && (binaryExpression.Operator == OperatorType.Plus || binaryExpression.Operator == OperatorType.Minus 
                || binaryExpression.Operator == OperatorType.Multiply || binaryExpression.Operator == OperatorType.Modulo))
            {
                var result = interpreter.Evaluate(binaryExpression);
                return result;
            }
            else return interpreter.Evaluate(statement);
        }

        // =============================================================================================================
        // HELPER :
        // =============================================================================================================

        /// <summary>
        /// Writes a signe lines using the given console <paramref name="color"/>, then restore the old color.
        /// </summary>
        /// <param name="color">New color to be used.</param>
        /// <param name="message">Message to be print.</param>
        private static void WriteLineWithColor(ConsoleColor color, string message)
        {
            var old = Console.ForegroundColor;
            try
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
            }
            finally
            {
                Console.ForegroundColor = old;
            }
        }

        /// <summary>
        /// Writes an information line (blue) in console.
        /// </summary>
        /// <param name="message">Message to be print.</param>
        private static void WriteInfoLine(string message) => WriteLineWithColor(ConsoleColor.Blue, message);

        /// <summary>
        /// Write an error line (red) in console.
        /// </summary>
        /// <param name="message">Message to be print.</param>
        private static void WriteErrorLine(string message) => WriteLineWithColor(ConsoleColor.Red, message);
    }
}
