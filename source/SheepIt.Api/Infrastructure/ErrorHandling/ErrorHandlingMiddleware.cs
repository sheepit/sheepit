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

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context, ErrorHandlingSettings settings)
        {
            try
            {
                await next(context);
            }
            catch (CustomException ex)
            {
                await HandleExceptionAsync(context, new ErrorResponse(ex, settings));
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, new ErrorResponse(ex, settings));
            }
        }

        private Task HandleExceptionAsync(HttpContext context, ErrorResponse errorResponse)
        {
            var code = HttpStatusCode.InternalServerError;

            
            var result = JsonConvert.SerializeObject(
                errorResponse,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }
            );
            
            Log.Error(result);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }
    }
}