using System;
using DecisionTelecom.Models;

namespace DecisionTelecom.Exceptions
{
    public class SmsException : Exception
    {
        public SmsErrorCode ErrorCode { get; }

        public bool HasErrorCode => (int)ErrorCode > 0;

        public SmsException(string message) : base(message)
        {
        }
        
        public SmsException(string message, Exception exception) : base(message, exception)
        {
        }
        
        public SmsException(string message, SmsErrorCode errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}