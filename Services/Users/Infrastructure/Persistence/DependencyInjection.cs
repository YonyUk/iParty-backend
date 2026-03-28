using Common.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Users.Application.Services;
using Users.Domain;
using Users.Domain.Rules;
using Users.Domain.Services;
using Users.Infrastructure.Configuration;
using Users.Infrastructure.Persistence;
using Users.Infrastructure.Providers;
using Users.Infrastructure.Security;

namespace Users.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceService(this IServiceCollection services,IConfiguration configuration)
    {
        services.Configure<UserDomainRulesOptions>(configuration.GetSection("UsersDomain"));
        services.Configure<DatabaseConfigOptions>(configuration.GetSection("Database"));
        services.AddScoped<IPasswordHasher,PasswordHasher>();
        services.AddScoped<IUserDomainRulesConfigProvider,UserDomainRulesConfigProvider>();
        services.InjectDatabaseService(configuration);
        services.AddScoped<IUserRepository,UserRepository>();
        services.AddScoped<IUnitOfWork,UnitOfWork>();
        services.AddScoped<IUserUniquenessChecker,UserUniquenessChecker>();
        return services;
    }

    static IServiceCollection InjectDatabaseService(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>((sp,options) =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var dbOptions = sp.GetRequiredService<IOptions<DatabaseConfigOptions>>().Value;

            options.UseNpgsql(connectionString,npgsqlOptions =>
            {
                npgsqlOptions.CommandTimeout(dbOptions.CommandTimeoutSeconds);
            });

            if (dbOptions.EnableSensitiveDataLogging)
                options.EnableSensitiveDataLogging();
            
        });
        return services;
    }
}