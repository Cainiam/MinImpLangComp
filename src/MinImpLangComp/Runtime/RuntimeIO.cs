using System;
using System.Text;

namespace MinImpLangComp.Runtime
{
    public static class RuntimeIO
    {
        private static readonly ThreadLocal<StringBuilder> _buffer = new ThreadLocal<StringBuilder>(() => new StringBuilder(), trackAllValues: false);
        private static StringBuilder Buffer => _buffer.Value!;

        public static void Print(object? value)
        {
            Buffer.AppendLine(value?.ToString() ?? string.Empty);
        }

        public static void Clear() => Buffer.Clear();

        public static string Consume()
        {
            var s = _buffer.ToString();
            Buffer.Clear();
            return s;
        }
    }
}
