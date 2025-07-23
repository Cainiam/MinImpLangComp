using System;
using MinImpLangComp.Lexing;
using MinImpLangComp.Parsing;
using MinImpLangComp.Exceptions;
using MinImpLangComp.Interpreting;
using MinImpLangComp.AST;

namespace MinImpLangComp.ReplLoop
{
    public static class Repl
    {
        public static void Run()
        {
            var interpreter = new Interpreter();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Bienvenue sur le REPL de MinImpLangComp. Ecrivez 'exit' pour quitter celui-ci lorsque vous avez terminé.");
            Console.ResetColor();
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
                    Console.WriteLine(" " + result);
                    Console.ResetColor();
                }
                catch (ParsingException pex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"/!\\ Syntax Error ~ {pex.Message}");
                    Console.ResetColor();
                }
                catch (RuntimeException rex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"/!\\ Runtime Error ~ {rex.Message}");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"/!\\ Unknow Error ~ {ex.Message}");
                    Console.ResetColor();
                }
            }
        }

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
    }
}
