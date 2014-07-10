using System;

namespace System
{
    /// <summary>
    /// Represents an error in the length of an argument to a method
    /// </summary>
    public class ArgumentLengthException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the System.ArgumentLengthException class.
        /// </summary>
        public ArgumentLengthException() : base() { }
        /// <summary>
        /// Initializes a new instance of the System.ArgumentLengthException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ArgumentLengthException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the System.ArgumentLengthException class with a predefined message based on
        /// <paramref name="argument"/> and <paramref name="length"/>.
        /// </summary>
        /// <param name="argument">Name of the method argument in error</param>
        /// <param name="length">Valid length of the argument</param>
        public ArgumentLengthException(string argument, int length) : base(String.Format("{0} length must be {1}.", argument, length)) { }
        /// <summary>
        /// Initializes a new instance of the System.ArgumentLengthException class with a formatted error message.
        /// See <see cref="String.Format(string, object[])"/>.
        /// </summary>
        /// <param name="format">format string</param>
        /// <param name="formatparams">format string substitutions</param>
        public ArgumentLengthException(string format, params object[] formatparams) : base(String.Format(format, formatparams)) { }
    }
}
