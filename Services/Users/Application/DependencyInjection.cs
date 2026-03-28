using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Users.Application.Commands.Validators;

namespace Users.Application.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // registers MediatR with all handlers from current assembly
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        // registers FluentValidation's validators from current Assembly
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        // registers all defined behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>),typeof(CommandValidationBehavior<,>));
        return services;
    }
}