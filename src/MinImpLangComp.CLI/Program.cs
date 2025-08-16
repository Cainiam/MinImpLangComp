using MinImpLangComp;
using MinImpLangComp.AST;
using MinImpLangComp.Facade;
using MinImpLangComp.ILGeneration;
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
                        Console.Error.WriteLine("Missing file path.");
                        return 2;
                    }
                    return RunFile(args[1]);
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
            Console.WriteLine("  1. micl run <file.mil>");
            Console.WriteLine("  2. micl repl");
            Console.ResetColor();
        }

        private static int RunFile(string path)
        {
            string source;
            try
            {
                source = File.ReadAllText(path);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine (e.Message);
                return 4;
            }
            try
            {
                var output = CompilerFacade.Run(source);
                if (!string.IsNullOrEmpty(output)) Console.Write(output);
                return 0;
            }
            catch (NotImplementedException ne)
            {
                Console.Error.WriteLine("Parser not wired yet: " + ne.Message);
                return 5;
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine("Runtime error: " + ex.Message);
                return 6;
            }
        }

        private static int RunRepl()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("> ");
            Console.ResetColor();
            while (true)
            {
                var line = Console.ReadLine();
                if (line == null || line.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase)) break;
                try
                {
                    var result = CompilerFacade.Run(line);
                    if(!string.IsNullOrEmpty(result)) Console.Write(result);
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
                MinImpLangComp.ReplLoop.Repl.Run();
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