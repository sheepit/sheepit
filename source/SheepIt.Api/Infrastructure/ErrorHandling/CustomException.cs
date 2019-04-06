using System;

namespace SheepIt.Api.Infrastructure.ErrorHandling
{
    public class CustomException : Exception
    {
        public int ErrorCode { get; }
        public string HumanReadableMessage { get; }

        public CustomException(int errorCode, string message, string humanReadableMessage)
            : base(message)
        {
            ErrorCode = errorCode;
            HumanReadableMessage = humanReadableMessage;
        }
    }
}