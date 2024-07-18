using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace ServiceQuotes.API.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app, IHostEnvironment environment)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                if (contextFeature != null)
                {
                    var logger = app.ApplicationServices.GetRequiredService<ILogger<Program>>();
                    logger.LogError(contextFeature.Error, "Unhandled exception occurred: {Trace}.", contextFeature.Error.StackTrace);


                    if (environment.IsDevelopment())
                    {
                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = contextFeature.Error.Message,
                            Trace = contextFeature.Error.StackTrace
                        }.ToString());
                    }
                    else
                    {
                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Um erro desconhecido ocorreu.",
                        }.ToString());
                    }
                };
            });
        });
    }
}
