using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceQuotes.Application.Exceptions;

namespace ServiceQuotes.API.Filters;

public class ApiExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ApiExceptionFilter> _logger;

    public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        if (context.Exception is ServiceQuoteException)
        {
            var serviceQuoteException = (ServiceQuoteException) context.Exception;

            context.HttpContext.Response.StatusCode = (int) serviceQuoteException.GetStatusCode();

            var response = new ResponseExceptionError(serviceQuoteException.GetErrorMessages());

            context.Result = new NotFoundObjectResult(response);

            _logger.LogWarning(context.Exception, "Erros: {Message}", context.Exception.Message);

        }
    }
}
