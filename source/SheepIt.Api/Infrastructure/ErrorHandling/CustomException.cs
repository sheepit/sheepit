using System;

namespace SheepIt.Api.Infrastructure.ErrorHandling
{
    public class CustomException : Exception
    {
        public string ErrorCode { get; }
        public string HumanReadableMessage { get; }

        public CustomException(string errorCode, string humanReadableMessage, Exception innerException)
            : base(humanReadableMessage, innerException)
        {
            ErrorCode = errorCode;
            HumanReadableMessage = humanReadableMessage;
        }
        
        public CustomException(string errorCode, string humanReadableMessage)
            : base($"{errorCode} - {humanReadableMessage}")
        {
            ErrorCode = errorCode;
            HumanReadableMessage = humanReadableMessage;
        }
    }
}