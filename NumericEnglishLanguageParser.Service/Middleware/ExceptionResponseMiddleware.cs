using System.Net;
using System.Net.Mime;
using Newtonsoft.Json;
using Serilog;
using ILogger = Serilog.ILogger;

namespace NumericEnglishLanguageParser.Service.Middleware;

/* SUMMARY:
 *
 *  Middleware to respond meaningfully to unhandled or thrown exceptions.
 *  This provides a single place to handle these exceptions without cluttering the controller code with exception handlers.
 *
 *  I've simplified the response output to the exception message but included the exception object in a Log.Error() call,
 *  meaning the stack trace is still accessible if needed.
 *
 */
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
        catch (Exception ex) when (ex is BadHttpRequestException or HttpRequestException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = MediaTypeNames.Application.Json;

            Log.Error(ex, $"Bad or malformed {context.Request.Method} request to {context.Request.Path}");

            await context.Response.WriteAsync(
                JsonConvert.SerializeObject(new
                {
                    exception = "Malformed request",
                    message = ex.Message
                })
            );
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = MediaTypeNames.Application.Json;

            Log.Error(ex, $"Unknown error occured in {context.Request.Method} request to {context.Request.Path}");
            
            await context.Response.WriteAsync(
                JsonConvert.SerializeObject(new
                {
                    exception = "Unknown exception",
                    message = ex.Message
                })
            );
        }
    }
}
