using System;
using System.Collections.Generic;

namespace SheepIt.Api.Infrastructure.ErrorHandling
{
    public class ErrorResponse
    {
        public int ErrorCode { get; }
        public string Type { get; }
        public string HumanReadableMessage { get; }
        public DeveloperDetails DeveloperDetails { get; }

        public ErrorResponse(Exception e)
        {
            ErrorCode = -1;
            Type = e.GetType().Name;
            HumanReadableMessage = "Server error occured";
            DeveloperDetails = new DeveloperDetails(e); // TODO: config setting to enable it
        }

        public ErrorResponse(CustomException e)
        {
            ErrorCode = e.ErrorCode;
            Type = e.GetType().Name;
            HumanReadableMessage = e.HumanReadableMessage;
            DeveloperDetails = new DeveloperDetails(e); // TODO: config setting to enable it
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