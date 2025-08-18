using MinImpLangComp.Facade;
using MinImpLangComp.ReplLoop;
using System.Globalization;

namespace MinImpLangComp.CLI
{ 
    /// <summary>
    /// Command-line netry point for MinImpLangComp.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Application entry point. Dispatches subcommands: run | samples | repl | repl-interp.
        /// </summary>
        /// <param name="args">CLI arguments.</param>
        /// <returns>Process exit code.</returns>
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
                    return HandleRun(args);
                case "samples":
                    return ListSamples();
                case "repl":
                    return RunRepl();
                case "repl-interp":
                    return RunInterpreterRepl();
                default:
                    Console.Error.WriteLine($"Unknown command '{args[0]}'.");
                    PrintUsage();
                    return 3;
            }
        }

        /// <summary>
        /// Prints usage information to stdout (with colored header).
        /// </summary>
        private static void PrintUsage()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Usage :");
            Console.WriteLine("  1. MinImpLangComp run <file.milc>");
            Console.WriteLine("  2. MinImpLangComp run -");
            Console.WriteLine("  3. MinImpLangComp run --pick   (interactive sample picker)");
            Console.WriteLine("  4. MinImpLangComp run sample:<name>");
            Console.WriteLine("  5. MinImpLangComp samples      (list available samples)");
            Console.WriteLine("  6. MinImpLangComp repl");
            Console.ResetColor();
        }

        /// <summary>
        /// Handles the <c>run</c> subcommand and its variants.
        /// </summary>
        /// <param name="args">CLI arguments.</param>
        /// <returns></returns>
        private static int HandleRun(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("Missing file path. Use a .milc file or '-' for stdin.");
                return 2;
            }
            if (args[1] == "--pick") return RunPickSample();
            if (args[1].StartsWith("sample:", StringComparison.OrdinalIgnoreCase))
            {
                var name = args[1].Substring("sample:".Length);
                var path = ResolveSampleByName(name);
                if (path == null)
                {
                    Console.Error.WriteLine($"Sample '{name}' not found.");
                    return 4;
                }
                return RunFileOrStdin(path);
            }
            return RunFileOrStdin(args[1]);
        }

        /// <summary>
        /// Runs a source provided via file path (ending with .milc) or STDIN (when <c>-</c>).
        /// Returns the corresponding exit codes.
        /// </summary>
        /// <param name="pathOrDash">File path (or -).</param>
        /// <returns>Corresponding exit code.</returns>
        private static int RunFileOrStdin(string pathOrDash)
        {
            string source;
            try
            {
                if (pathOrDash == "-")
                {
                    if (!Console.IsInputRedirected)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Reading from stdin. Paste your program then end with:");
                        Console.WriteLine("  - Windows : Ctrl+Z then Enter");
                        Console.WriteLine("  - Linux/Mac : Ctrl+D");
                        Console.ResetColor();
                    }
                    source = Console.In.ReadToEnd();
                }
                else
                {
                    if (!pathOrDash.EndsWith(".milc", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.Error.WriteLine("Expected a .milc file (or '-' for stdin).");
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
                var output = CompilerFacade.Run(source);
                if (!string.IsNullOrEmpty(output)) Console.Write(output);
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

        /// <summary>
        /// Start the IL REPL loop (Ctrl+C to abort). Returns 0 on normal exit.
        /// </summary>
        /// <returns>0 on normal exit.</returns>
        private static int RunRepl()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("MinImpLangComp IL REPL - type 'exit' to quit.");
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
                    var output = CompilerFacade.Run(line);
                    if (!string.IsNullOrEmpty (output)) Console.Write(output);
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
        
        /// <summary>
        /// Starts the legacy interpreter REPL. Returns specific error code on failure.
        /// </summary>
        /// <returns>0 on normal exit.</returns>
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

        /// <summary>
        /// Interactive sample picker, then executes the chosen samples.
        /// </summary>
        /// <returns>Execute the choosen sample. 0 on normal exit.</returns>
        private static int RunPickSample()
        {
            var files = GetSampleFiles();
            if (files.Length == 0)
            {
                Console.Error.WriteLine("No sample found.");
                return 4;
            }
            Console.WriteLine("Available samples:");
            for(int i = 0; i < files.Length; i++)
            {
                Console.WriteLine($"  {i+1}. {Path.GetFileName(files[i])}");
            }
            Console.Write("Choose a number (or press Enter for 1): ");
            var choice = Console.ReadLine();
            int index = 1;
            if (!string.IsNullOrWhiteSpace(choice) && !int.TryParse(choice, out index))
            {
                Console.Error.WriteLine("Invalid selection.");
                return 4;
            }
            if (index < 1 || index > files.Length)
            {
                Console.Error.WriteLine("Selection out of range.");
                return 4;
            }
            return RunFileOrStdin(files[index - 1]);
        }

        /// <summary>
        /// List available sample files to stdout. Returns 0 even when none exist.
        /// </summary>
        /// <returns>List of samples. 0 on exit.</returns>
        private static int ListSamples()
        {
            var files = GetSampleFiles();
            if (files.Length == 0)
            {
                Console.WriteLine("No samples found.");
                return 0;
            }
            foreach (var file in files) Console.WriteLine(Path.GetFileName(file));
            return 0;
        }

        /// <summary>
        /// Resolves a sample by name (with or without .milc extension).
        /// </summary>
        /// <param name="name">Sample name.</param>
        /// <returns>Sample name path or null.</returns>
        private static string? ResolveSampleByName(string name)
        {
            var files = GetSampleFiles();
            foreach (var file in files)
            {
                var filename = Path.GetFileName(file);
                if (filename.Equals(name, StringComparison.OrdinalIgnoreCase)) return file;
                if (Path.GetFileNameWithoutExtension(filename).Equals(name, StringComparison.OrdinalIgnoreCase)) return file;
            }
            return null;
        }

        /// <summary>
        /// Find sample files, honoring the MINIMPLANGCOMP_SAMPLES_DIR override, or walking up from AppContext.BaseDirectory.
        /// </summary>
        /// <returns>Sample files path.</returns>
        private static string[] GetSampleFiles()
        {
            var overrideDir = Environment.GetEnvironmentVariable("MINIMPLANGCOMP_SAMPLES_DIR");
            if (!string.IsNullOrWhiteSpace(overrideDir) && Directory.Exists(overrideDir)) return Directory.GetFiles(overrideDir, "*.milc", SearchOption.TopDirectoryOnly);
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while(dir != null)
            {
                var candidate = Path.Combine(dir.FullName, "samples");
                if (Directory.Exists(candidate)) return Directory.GetFiles(candidate, "*.milc", SearchOption.TopDirectoryOnly);
                dir = dir.Parent;
            }
            return Array.Empty<string>();
        }
    }
}