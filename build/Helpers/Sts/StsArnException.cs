using System;

namespace Build.Aws
{
    /// <summary>
    /// The exception is thrown when an error occurs when retrieving ARN from Sts.
    /// </summary>
    public class StsArnException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the StsArnException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public StsArnException(string message) : base(message)
        {
        }
    }
}