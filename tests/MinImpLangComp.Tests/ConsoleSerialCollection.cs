using Xunit;

namespace MinImpLangComp.Tests
{
    /// <summary>
    /// xUnit test collection used to serialize tests that interact with console I/O
    /// <para>
    /// By disabling parallelization, we avoid contention on global state such as <see cref="System.Console"/>, environnement variables, and thread-local buffers.
    /// </para>
    /// </summary>
    [CollectionDefinition("ConsoleSerial", DisableParallelization = true)]
    public class ConsoleSerialCollection : ICollectionFixture<object> 
    { 
        // No Code needed.
    }
}
