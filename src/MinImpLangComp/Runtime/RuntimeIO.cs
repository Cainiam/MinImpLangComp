using System;
using System.Text;

namespace MinImpLangComp.Runtime
{
    public static class RuntimeIO
    {
        private static readonly StringBuilder _buffer = new StringBuilder();

        public static void Print(object? value)
        {
            Console.WriteLine(value);
            _buffer.AppendLine(value?.ToString() ?? string.Empty);
        }

        public static void Clear() => _buffer.Clear();

        public static string Consume()
        {
            var s = _buffer.ToString();
            _buffer.Clear();
            return s;
        }
    }
}
