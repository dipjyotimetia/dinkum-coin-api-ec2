using System;

namespace Build.Aws
{
    /// <summary>
    /// The exception is thrown when an operation dealing with parameters in the EC2 Parameter Store fails.
    /// </summary>
    public class ParameterOperationException : Exception
    {
        /// <summary>
        /// Initialises a new instance of the ParameterOperationException class with the specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ParameterOperationException(string message) : base(message)
        {
        }
    }
}