using System;
using System.Runtime.Serialization;

namespace MinImpLangComp.Exceptions
{
    /// <summary>
    /// Control-flow exception used by the interpreter to unwind a function call and carry the returned value.
    /// </summary>
    [Serializable]
    public class ReturnException : Exception
    {
        /// <summary>
        /// The value carried by the return statement.
        /// </summary>
        public object ReturnValue { get; }

        /// <summary>
        /// Creates a new <see cref="ReturnException"/> carrying the specified return value.
        /// </summary>
        /// <param name="returnValue">The value to return (may be null).</param>
        public ReturnException(object returnValue) : base("Function returned.") 
        {
            ReturnValue = returnValue;
        }

        /// <summary>
        /// Creates a new <see cref="ReturnException"/>.
        /// </summary>
        public ReturnException() { }

        /// <summary>
        /// Creates a new <see cref="ReturnException"/> with a message.
        /// </summary>
        public ReturnException(string message) : base(message) { }

        /// <summary>
        /// Creates a new <see cref="ReturnException"/> with a message and inner exception.
        /// </summary>
        public ReturnException(string? message, Exception? innerException) : base(message, innerException) { }

        /// <summary>
        /// Serializable constructor.
        /// </summary>
        protected ReturnException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ReturnValue = info.GetValue(nameof(ReturnValue), typeof(object));
        }

        /// <summary>
        /// Adds custom data to the serialization stream.
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(ReturnValue), ReturnValue, typeof(object));
        }
    }
}
