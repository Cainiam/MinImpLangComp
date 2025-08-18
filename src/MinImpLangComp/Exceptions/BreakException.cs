using System;
using System.Runtime.Serialization;

namespace MinImpLangComp.Exceptions
{
    /// <summary>
    /// Control-flow exception used by the interpreter to break out of loop construct.
    /// </summary>
    [Serializable]
    public class BreakException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="BreakException"/>.
        /// </summary>
        public BreakException() { }

        /// <summary>
        /// Creates a new <see cref="BreakException"/> with a message.
        /// </summary>
        public BreakException(string message) : base(message) { }

        /// <summary>
        /// Creates a new <see cref="BreakException"/> with a message and inner exception.
        /// </summary>
        public BreakException(string? message, Exception? innerException) : base(message, innerException) { }

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        protected BreakException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
