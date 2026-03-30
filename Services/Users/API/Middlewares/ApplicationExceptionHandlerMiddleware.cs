using Microsoft.AspNetCore.Mvc;
using Users.Application.Exceptions;
using Users.Domain.Exceptions;

namespace Users.API.Middlewares;

public class ApplicationExceptionHandlerMiddleware : IMiddleware
{
    private readonly ILogger<ApplicationExceptionHandlerMiddleware> logger;
    private static readonly Dictionary<Type, int> ExceptionStatusCodeMap = new()
    {
        {typeof(RequiredFieldException),StatusCodes.Status400BadRequest},
        {typeof(InvalidUserRoleException),StatusCodes.Status400BadRequest},
        {typeof(InvalidEmailException),StatusCodes.Status400BadRequest},
        {typeof(InvalidPasswordException),StatusCodes.Status400BadRequest},
        {typeof(InvalidUserNameException),StatusCodes.Status400BadRequest},
        {typeof(InvalidHashedPasswordException),StatusCodes.Status500InternalServerError},
        {typeof(UserAlreadyExistsException),StatusCodes.Status409Conflict},
        {typeof(UserNotFoundException),StatusCodes.Status404NotFound}
    };
    public ApplicationExceptionHandlerMiddleware(ILogger<ApplicationExceptionHandlerMiddleware> logger)
    {
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleApplicationException(context,ex);
        }
    }
    private async Task HandleApplicationException(HttpContext context,Exception exception)
    {
        try
        {
            var statusCode = ExceptionStatusCodeMap[exception.GetType()];
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = statusCode;
            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = exception.GetType().Name,
                Detail = exception.Message,
                Instance = context.Request.Path
            };
            await context.Response.WriteAsJsonAsync(problem);
        }
        catch (Exception e)
        {
            logger.LogError(e,"Exception not controlled");
        }
    }
}