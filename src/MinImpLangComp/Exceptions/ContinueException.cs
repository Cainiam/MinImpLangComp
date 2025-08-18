using System;
using System.Runtime.Serialization;

namespace MinImpLangComp.Exceptions
{
    /// <summary>
    /// Control-flow exception used by the interpreter to jump to the next loop iteration.
    /// </summary>
    [Serializable]
    public class ContinueException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="ContinueException"/>.
        /// </summary>
        public ContinueException() { }

        /// <summary>
        /// Creates a new <see cref="ContinueException"/> with a message.
        /// </summary>
        public ContinueException(string message) { }

        /// <summary>
        /// Creates a new <see cref="ContinueException"/> with a message and inner exception.
        /// </summary>
        public ContinueException (string message, Exception innerException) { }

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        protected ContinueException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
