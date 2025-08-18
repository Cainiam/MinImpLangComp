using MinImpLangComp.Facade;
using Xunit;

namespace MinImpLangComp.Tests
{
    /// <summary>
    /// Tests that exercise the facade against real sample files (no CLI involved here).
    /// </summary>
    public class CLIFacadeTests
    {
        #region Helper
        /// <summary>
        /// Normalizes newlines to '\n' to make assertions platform-agnostic.
        /// </summary>
        private static string Normalize(string s) => s.Replace("\r\n", "\n").Replace("\r", "\n");

        /// <summary>
        /// Walks up for AppContext.BaseDirectory to find the 'samples' directory.
        /// </summary>
        private static string FindSamplesDir()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                var candidate = Path.Combine(dir.FullName, "samples");
                if (Directory.Exists(candidate)) return candidate;
                dir = dir.Parent;
            }
            throw new DirectoryNotFoundException("Directory 'samples' was not found");
        }

        /// <summary>
        /// Loads a sample file's source text by name for the discovered 'samples' folder
        /// </summary>
        /// <param name="fileName"></param>
        private static string LoadSample(string fileName)
        {
            var samples = FindSamplesDir();
            var path = Path.Combine(samples, fileName);
            if (!File.Exists(path)) throw new FileNotFoundException($"This sample does not exist: {path}");
            return File.ReadAllText(path);
        }
        #endregion

        /// <summary>
        /// Loads sample "hello.milc" and executes.
        /// </summary>
        [Fact]
        public void HelloSample_PrintsHello()
        {
            // Arange
            var source = LoadSample("hello.milc");

            // Act
            var output = CompilerFacade.Run(source);

            //Assert
            Assert.Equal("Hello!\n", Normalize(output));
        }

        /// <summary>
        /// Loads sample "ret.milc" and executes.
        /// </summary>
        [Fact]
        public void RetSample_Prints8()
        {
            var source = LoadSample("ret.milc");
            var output = CompilerFacade.Run(source);

            Assert.Equal("8\n", Normalize(output));
        }

        /// <summary>
        /// Loads sample "sum.milc" and executes.
        /// </summary>
        [Fact]
        public void SumSample_Prints12()
        {
            var source = LoadSample("sum.milc");
            var output = CompilerFacade.Run(source);

            Assert.Equal("12\n", Normalize(output));
        }
    }

    /// <summary>
    /// CLI-level tests: execute Program.Main with arguments while capturing stdio.
    /// </summary>
    [Collection("ConsoleSerial")]
    public class CLIFacadeCommandsTests
    {
        #region Helper Commands
        /// <summary>
        /// Walks up for AppContext.BaseDirectory to find the 'samples' directory.
        /// </summary>
        private static string FindSamplesDir()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while(dir != null)
            {
                var candidate = Path.Combine(dir.FullName, "samples");
                if (Directory.Exists(candidate)) return candidate;
                dir = dir.Parent!;
            }
            throw new DirectoryNotFoundException("Directory 'samples' was not found");
        }

        /// <summary>
        /// Runs the CLI entry point with given args and optional stdin, capturing stdout.
        /// Also inject the 'MINIMPLANGCOMP_SAMPLES_DIR' env var so the LCI resolves samples deterministically.
        /// </summary>
        private static string CaptureConsole(string[] args,  out int exitCode, string? stdin = null)
        {
            var originalIn = Console.In;
            var originalOut = Console.Out;
            const string EnvName = "MINIMPLANGCOMP_SAMPLES_DIR";
            var originalEnv = Environment.GetEnvironmentVariable(EnvName);
            try
            {
                Environment.SetEnvironmentVariable(EnvName, FindSamplesDir());
                if (stdin != null) Console.SetIn(new StringReader(stdin));
                using var sw = new StringWriter();
                Console.SetOut(sw);

                // Execute the CLI entry point
                int code = CLI.Program.Main(args);
                exitCode = code;

                // Normalize newlines + trim trailing newline to simplify assertions
                return sw.ToString().Replace("\r\n", "\n").Replace("\r", "\n").TrimEnd('\n');
            }
            finally
            {
                // Restore console + environment
                Console.SetIn(originalIn);
                Console.SetOut(originalOut);
                Environment.SetEnvironmentVariable("MINIMPLANG_SAMPLES_DIR", originalEnv);
            }
        }
        #endregion

        /// <summary>
        /// Check if command print list of samples.
        /// </summary>
        [Fact]
        public void SamplesCommand_ListFiles()
        {
            // Act
            int code;
            var stdout = CaptureConsole(new[] { "samples" }, out code);

            // Assert
            Assert.Equal(0, code);
            Assert.Contains("hello.milc", stdout);
            Assert.Contains("ret.milc", stdout);
            Assert.Contains("sum.milc", stdout);
        }

        /// <summary>
        /// Check if command executes file by name
        /// </summary>
        [Fact]
        public void RunSample_ByName_HelloPrintsHello()
        {
            int code;
            var stdout= CaptureConsole(new[] { "run", "sample:hello" }, out code);

            Assert.Equal(0, code);
            Assert.Equal("Hello!", stdout);
        }

        /// <summary>
        /// Check if simulated input executes choosen command.
        /// </summary>
        [Fact]
        public void RunPick_Interactive_ChoosesFirstAndRuns()
        {
            // Simulate choosing index 1 (pressing "1" then Enter).
            int code;
            var stdout = CaptureConsole(new[] { "run", "--pick" }, out code, stdin: "1\n");

            Assert.Equal(0, code);

            // Extract the payload after the pickerUI.
            // Strategy: take substring after the last colon (if any), otherwise fallback to last non-empty line.
            var afterColonIdx = stdout.LastIndexOf(':');
            string payload;
            if (afterColonIdx >= 0 && afterColonIdx + 1 < stdout.Length) payload = stdout.Substring(afterColonIdx + 1).Trim();
            else
            {
                var lines = stdout.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                payload = lines.Length > 0 ? lines[^1].Trim() : "";
            }

            Assert.True(payload == "Hello!" || payload == "8" || payload == "12", $"Unexpected output for --pick '{stdout}' (extracted payload: '{payload}')");
        }
    }
}
