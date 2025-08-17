using System;
using System.Text;
using System.Threading;

namespace MinImpLangComp.Runtime
{
    /// <summary>
    /// Process-local I/O buffer used by the runtime to capture printed output.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Uses a <see cref="ThreadLocal{T}"/> <see cref="StringBuilder"/> so parallel test runs do not interfere with each other's output.
    /// </para>
    /// <para>
    /// Contract:
    /// <list type="bullet">
    /// <item><description><see cref="Print(object?)"/> appends a line (value.ToString() or empty if null).</description></item>
    /// <item><description><see cref="Consume"/> returns the full buffered output and clears the buffer.</description></item>
    /// <item><description><see cref="Clear"/> clears without returning.</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public static class RuntimeIO
    {
        private static readonly ThreadLocal<StringBuilder> _buffer = new ThreadLocal<StringBuilder>(() => new StringBuilder(), trackAllValues: false);

        /// <summary>
        /// Gets the current thread's buffer, creating it if necessary.
        /// </summary>
        private static StringBuilder Buffer => _buffer.Value!;

        /// <summary>
        /// Appends a line to the buffer. If <paramref name="value"/> is null, appends an empty line.
        /// </summary>
        /// <param name="value">Object to be printed.</param>
        public static void Print(object? value)
        {
            Buffer.AppendLine(value?.ToString() ?? string.Empty);
        }

        /// <summary>
        /// Clears the current thread's buffer.
        /// </summary>
        public static void Clear() => Buffer.Clear();

        /// <summary>
        /// Returns the buffered content and clears the buffer.
        /// </summary>
        /// <returns>Buffered content.</returns>
        public static string Consume()
        {
            var b = Buffer;
            var s = b.ToString();
            b.Clear();
            return s;
        }
    }
}
