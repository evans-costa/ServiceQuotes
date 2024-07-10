using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ServiceQuotes.Filters;

public class ApiExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ApiExceptionFilter> _logger;

    public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Um erro desconhecido ocorreu.");

        context.Result = new ObjectResult("Um erro desconhecido ocorreu.")
        {
            StatusCode = StatusCodes.Status500InternalServerError,
        };
    }
}
