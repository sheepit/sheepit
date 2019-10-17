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

        public ErrorResponse(Exception e, ErrorHandlingSettings settings)
        {
            ErrorCode = "-1";
            Type = e.GetType().Name;
            HumanReadableMessage = "Server error occured";

            if(settings.DeveloperDetails)
                DeveloperDetails = new DeveloperDetails(e);
        }

        public ErrorResponse(CustomException e, ErrorHandlingSettings settings)
        {
            ErrorCode = e.ErrorCode;
            Type = e.GetType().Name;
            HumanReadableMessage = e.HumanReadableMessage;

            if(settings.DeveloperDetails)
                DeveloperDetails = new DeveloperDetails(e);
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