using System;
using System.Runtime.Serialization;

namespace MinImpLangComp.Exceptions
{
    /// <summary>
    /// Exception thrown when a runtime error occurs during program execution.
    /// </summary>
    [Serializable]
    public class RuntimeException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="RuntimeException"/>.
        /// </summary>
        public RuntimeException() { }

        /// <summary>
        /// Creates a new <see cref="RuntimeException"/> with a message.
        /// </summary>
        /// <param name="message">Describes the runtime error.</param>
        public RuntimeException(string message) : base(message) { }

        /// <summary>
        /// Creates a new <see cref="RuntimeException"/> with a message and an inner exception.
        /// </summary>
        /// <param name="message">Describes the runtime error.</param>
        /// <param name="innerException">Inner exception providing additional context.</param>
        public RuntimeException(string? message, Exception? innerException) : base(message, innerException) { }

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        protected RuntimeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
