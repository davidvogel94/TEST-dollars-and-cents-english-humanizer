using System.Net;
using System.Net.Mime;
using Newtonsoft.Json;
using Serilog;
using ILogger = Serilog.ILogger;

namespace NumericEnglishLanguageParser.Service.Middleware
{
    public class ExceptionResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionResponseMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            // TODO: catch more specific error types

            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new { message = ex.Message }));
            }
        }
    }
}