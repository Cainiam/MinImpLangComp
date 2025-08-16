using Microsoft.VisualStudio.TestPlatform.TestHost;
using MinImpLangComp.Facade;
using Xunit;

namespace MinImpLangComp.Tests
{
    public class CLIFacadeTests
    {
        private static string Normalize(string s) => s.Replace("\r\n", "\n").Replace("\r", "\n");

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

        private static string LoadSample(string fileName)
        {
            var samples = FindSamplesDir();
            var path = Path.Combine(samples, fileName);
            if (!File.Exists(path)) throw new FileNotFoundException($"This sample does not exist: {path}");
            return File.ReadAllText(path);
        }

        [Fact]
        public void HelloSample_PrintsHello()
        {
            var source = LoadSample("hello.milc");
            var output = CompilerFacade.Run(source);

            Assert.Equal("Hello!\n", Normalize(output));
        }

        [Fact]
        public void RetSample_Prints8()
        {
            var source = LoadSample("ret.milc");
            var output = CompilerFacade.Run(source);

            Assert.Equal("8\n", Normalize(output));
        }

        [Fact]
        public void SumSample_Prints12()
        {
            var source = LoadSample("sum.milc");
            var output = CompilerFacade.Run(source);

            Assert.Equal("12\n", Normalize(output));
        }
    }

    [Collection("ConsoleSerial")]
    public class CLIFacadeCommandsTests
    {
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

        private static string CaptureConsole(string[] args,  out int exitCode, string? stdin = null)
        {
            var originalIn = Console.In;
            var originalOut = Console.Out;
            var originalEnv = Environment.GetEnvironmentVariable("MINIMPLANG_SAMPLES_DIR");
            try
            {
                Environment.SetEnvironmentVariable("MINIMPLANG_SAMPLES_DIR", FindSamplesDir());
                if (stdin != null) Console.SetIn(new StringReader(stdin));
                using var sw = new StringWriter();
                Console.SetOut(sw);
                int code = CLI.Program.Main(args);
                exitCode = code;
                return sw.ToString().Replace("\r\n", "\n").Replace("\r", "\n").TrimEnd('\n');
            }
            finally
            {
                Console.SetIn(originalIn);
                Console.SetOut(originalOut);
                Environment.SetEnvironmentVariable("MINIMPLANG_SAMPLES_DIR", originalEnv);
            }
        }

        [Fact]
        public void SamplesCommand_ListFiles()
        {
            int code;
            var stdout = CaptureConsole(new[] { "samples" }, out code);

            Assert.Equal(0, code);
            Assert.Contains("hello.milc", stdout);
            Assert.Contains("ret.milc", stdout);
            Assert.Contains("sum.milc", stdout);
        }

        [Fact]
        public void RunSample_ByName_HelloPrintsHello()
        {
            int code;
            var stdout= CaptureConsole(new[] { "run", "sample:hello" }, out code);

            Assert.Equal(0, code);
            Assert.Equal("Hello!", stdout);
        }

        [Fact]
        public void RunPick_Interactive_ChoosesFirstAndRuns()
        {
            int code;
            var stdout = CaptureConsole(new[] { "run", "--pick" }, out code, stdin: "1\n");

            Assert.Equal(0, code);

            var afterColonIdx = stdout.LastIndexOf(':');
            string payload;
            if (afterColonIdx >= 0 && afterColonIdx + 1 < stdout.Length) payload = stdout.Substring(afterColonIdx + 1).Trim();
            else
            {
                var lines = stdout.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                payload = lines.Length > 0 ? lines[^1].Trim() : "";
            }

            Assert.True(payload == "Hello!" || payload == "8" || payload == "12", $"Unexpected output for --pick '{stdout}' (extracted payloard: '{payload}')");
        }
    }
}
