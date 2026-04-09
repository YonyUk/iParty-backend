<<<<<<< HEAD
using System.Net;
=======
>>>>>>> dev-users-api
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Users.Application.Exceptions;
using Users.Domain.Exceptions;

namespace Users.API.Middlewares;

public class ApplicationExceptionHandlerMiddleware : IMiddleware
{
    private readonly ILogger<ApplicationExceptionHandlerMiddleware> logger;
    private static readonly Dictionary<Type, int> ExceptionStatusCodeMap = new()
    {
        {typeof(ValidationException),StatusCodes.Status400BadRequest},
        {typeof(RequiredFieldException),StatusCodes.Status400BadRequest},
        {typeof(InvalidUserRoleException),StatusCodes.Status400BadRequest},
        {typeof(InvalidEmailException),StatusCodes.Status400BadRequest},
        {typeof(InvalidPasswordException),StatusCodes.Status400BadRequest},
        {typeof(InvalidUserNameException),StatusCodes.Status400BadRequest},
        {typeof(InvalidHashedPasswordException),StatusCodes.Status500InternalServerError},
        {typeof(UserAlreadyExistsException),StatusCodes.Status409Conflict},
        {typeof(UserNotFoundException),StatusCodes.Status404NotFound},
        {typeof(ValidationException),StatusCodes.Status400BadRequest}
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
            await HandleApplicationException(context, ex);
        }
    }
    private async Task HandleApplicationException(HttpContext context, Exception exception)
    {
        if (!context.Response.HasStarted)
            context.Response.Clear();
        try
        {
            var statusCode = ExceptionStatusCodeMap[exception.GetType()];
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = statusCode;

            if (exception is ValidationException)
            {
                var errorsList = ((ValidationException)exception).Errors
                    .Select(
                        e => new KeyValuePair<string, string[]>(
                            e.PropertyName,
                            new[] { e.ErrorMessage }
                        )
                    );
                
                var errors = new Dictionary<string,string[]>();
                foreach(var error in errorsList)
                {
                    if (!errors.ContainsKey(error.Key))
                        errors[error.Key] = error.Value;
                    else
                        errors[error.Key] = errors[error.Key].Append(error.Value[0]).ToArray();
                }
                var validationProblemDetails = new ValidationProblemDetails(errors);
                await context.Response.WriteAsJsonAsync(validationProblemDetails);
                return;
            }
            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = exception.GetType().Name,
                Detail = exception.Message,
                Instance = context.Request.Path,
            };
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(problem);

        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception not controlled");
            var problem = new ProblemDetails
            {
                Status = 500,
                Title = "InternalServerError",
                Detail = "An unexpected error has occurred",
                Instance = context.Request.Path
            };
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}