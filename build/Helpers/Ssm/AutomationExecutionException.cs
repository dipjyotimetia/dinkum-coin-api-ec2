using System;

namespace Build.Aws
{
    /// <summary>
    /// The exception is thrown when the execution of an automatic document fails.
    /// </summary>
    public class AutomationExecutionException : Exception
    {
        /// <summary>
        /// Initialises a new instance of the ParameterOperationException class with the specified error message and automation execution ID.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="automationExecutionId">The ID of the automation execution that has failed.</param>
        public AutomationExecutionException(string message, string automationExecutionId) : base(message)
        {
            AutomationExecutionId = automationExecutionId;
        }

        /// <summary>
        /// The ID of the automation execution that has failed.
        /// </summary>
        public string AutomationExecutionId { get; }
    }
}