using System;
using System.Runtime.Serialization;

namespace MinImpLangComp.Exceptions
{
    /// <summary>
    /// Exception trhown when a parsing error is encountered.
    /// </summary>
    [Serializable]
    public class ParsingException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="ParsingException"/>.
        /// </summary>
        public ParsingException() { }

        /// <summary>
        /// Creates a new <see cref="ParsingException"/> with a message.
        /// </summary>
        /// <param name="message">Error message describing the parsing issue.</param>
        public ParsingException(string message) : base(message) { }

        /// <summary>
        /// Creates a new <see cref="ParsingException"/> with a message and inner exception.
        /// </summary>
        /// <param name="message">Error message describing the parsing issue.</param>
        /// <param name="innerException">Inner exception providing additional context.</param>
        public ParsingException(string? message, Exception? innerException) : base(message, innerException) { }

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        protected ParsingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
