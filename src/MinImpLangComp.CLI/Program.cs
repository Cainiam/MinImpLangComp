using MinImpLangComp.AST;
using MinImpLangComp.Facade;
using MinImpLangComp.ReplLoop;
using System.Globalization;

namespace MinImpLangComp.CLI
{ 
    public static class Program
    {
        public static int Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            if (args.Length == 0)
            {
                PrintUsage();
                return 1;
            }

            switch (args[0])
            {
                case "run":
                    if (args.Length < 2)
                    {
                        Console.Error.WriteLine("Missing file path. Use a .milc file or '-' for stdin.");
                        return 2;
                    }
                    return RunFileOrStdin(args[1]);
                case "repl":
                    return RunRepl();
                case "repl-interp":
                    return RunInterpreterRepl();
                default:
                    Console.Error.WriteLine($"Unknow command '{args[0]}'.");
                    PrintUsage();
                    return 3;
            }
        }

        private static void PrintUsage()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Usage :");
            Console.WriteLine("  1. MinImpLangComp run <file.milc>");
            Console.WriteLine("  2. MinImpLangComp run -");
            Console.WriteLine("  3. MinImpLangComp repl");
            Console.ResetColor();
        }

        private static int RunFileOrStdin(string pathOrDash)
        {
            string source;
            try
            {
                if (pathOrDash == "-") source = Console.In.ReadToEnd();
                else
                {
                    if (!pathOrDash.EndsWith(".milc", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.Error.WriteLine("Expected a  .milc file (or '-' for stdin).");
                        return 4;
                    }
                    source = File.ReadAllText(pathOrDash);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return 4;
            }
            try
            {
                _ = CompilerFacade.Run(source);
                return 0;
            }
            catch (NotImplementedException ne)
            {
                Console.Error.WriteLine("Parser not wired yet: " + ne.Message);
                return 5;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Runtime error: " + ex.Message);
                return 6;
            }
        }

        private static int RunRepl()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("MinImpLangComp IL REPL - type 'exit' to quit.");
            Console.ResetColor();
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("> ");
                Console.ResetColor();
                var line = Console.ReadLine();
                if (line == null || line.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase)) break;
                try
                {
                    _ = CompilerFacade.Run(line);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                }
            }
            return 0;
        }

        private static int RunInterpreterRepl()
        {
            try
            {
                Repl.Run();
                return 0;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return 7;
            }
        }
    }
}