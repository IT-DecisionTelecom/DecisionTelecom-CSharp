using System;
using System.Text.Json;
using DecisionTelecom.Models;

namespace DecisionTelecom.Exceptions
{
    /// <summary>
    /// Represents error which may occur while working with Viber messages
    /// </summary>
    public class ViberException : Exception
    {
        public ViberError Error { get; }

        public ViberException(string message) : base(message)
        {
        }
        
        public ViberException(string message, Exception exception) : base(message, exception)
        {
        }

        private ViberException(string message, ViberError error) : base(message)
        {
            Error = error;
        }

        public static ViberException FromResponse(string response)
        {
            var error = JsonSerializer.Deserialize<ViberError>(response);
            return new ViberException("An error occurred while processing request", error);
        }
    }
}