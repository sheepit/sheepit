using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;

namespace SheepIt.Api.Infrastructure.ErrorHandling
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        private readonly int _customHttpStatusCode = 555;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        // ReSharper disable once UnusedMember.Global - it's called by reflection
        public async Task Invoke(HttpContext context, ErrorHandlingSettings settings)
        {
            try
            {
                await next(context);
            }
            catch (CustomException exception)
            {
                await HandleExceptionAsync(
                    context: context,
                    httpStatusCode: _customHttpStatusCode,
                    errorResponse: new ErrorResponse(exception, settings)
                );
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(
                    context: context,
                    httpStatusCode: (int)HttpStatusCode.InternalServerError,
                    errorResponse: new ErrorResponse(exception, settings)
                );
            }
        }

        private Task HandleExceptionAsync(
            HttpContext context,
            int httpStatusCode,
            ErrorResponse errorResponse)
        {
            var result = JsonConvert.SerializeObject(
                errorResponse,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }
            );
            
            // todo: [rt] why log json, instead of an exception as-is?
            // todo: [rt] should we even log custom exceptions?
            Log.Error(result);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = httpStatusCode;

            return context.Response.WriteAsync(result);
        }
    }
}