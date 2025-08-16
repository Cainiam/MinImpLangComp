using MinImpLangComp.Facade;
using Xunit;

namespace MinImpLangComp.Tests
{
    public class CLIFacadeTests
    {
        private static string Normalize(string s) => s.Replace("\r\n", "\n").Replace("\r", "\n");

        private static string FindSmaplesDir()
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
            var samples = FindSmaplesDir();
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
}
