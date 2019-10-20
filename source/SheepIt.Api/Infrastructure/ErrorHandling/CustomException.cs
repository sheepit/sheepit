using System;

namespace SheepIt.Api.Infrastructure.ErrorHandling
{
    public class CustomException : Exception
    {
        public string ErrorCode { get; }
        public string HumanReadableMessage { get; }

        public CustomException(string errorCode, string humanReadableMessage)
            : base($"{errorCode} - {humanReadableMessage}")
        {
            ErrorCode = errorCode;
            HumanReadableMessage = humanReadableMessage;
        }
    }
}