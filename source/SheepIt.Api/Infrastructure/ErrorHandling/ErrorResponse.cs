using System;
using System.Collections.Generic;

namespace SheepIt.Api.Infrastructure.ErrorHandling
{
    public class ErrorResponse
    {
        public string ErrorCode { get; }
        public string Type { get; }
        public string HumanReadableMessage { get; }
        public DeveloperDetails DeveloperDetails { get; }
        public bool IsCustom { get; set; }

        public ErrorResponse(Exception exception, ErrorHandlingSettings settings)
        {
            ErrorCode = "-1";
            Type = exception.GetType().Name;
            HumanReadableMessage = "Server error occured";

            if (settings.DeveloperDetails)
            {
                DeveloperDetails = new DeveloperDetails(exception);
            }
            
            IsCustom = false;
        }

        public ErrorResponse(CustomException exception, ErrorHandlingSettings settings)
        {
            ErrorCode = exception.ErrorCode;
            Type = exception.GetType().Name;
            HumanReadableMessage = exception.HumanReadableMessage;

            if (settings.DeveloperDetails)
            {
                DeveloperDetails = new DeveloperDetails(exception);
            }
            
            IsCustom = true;
        }
    }

    public class DeveloperDetails
    {
        public List<string> Messages { get; }
        public string StackTrace { get; }
        
        public DeveloperDetails(Exception ex)
        {
            Messages = new List<string> { ex.Message };

            var inner = ex.InnerException;

            while (inner != null)
            {
                Messages.Add(inner.Message);
                inner = inner.InnerException;
            }

            StackTrace = ex.StackTrace;
        }
    }
}