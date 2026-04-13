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
using Users.Infrastructure.Services.Security;
using Users.Infrastructure.Persistence.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Npgsql;

namespace Users.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceService(this IServiceCollection services,IConfiguration configuration)
    {
        var securitySection = configuration.GetSection("Security");
        services.Configure<UserDomainRulesOptions>(opt => configuration.GetSection("UsersDomain").Bind(opt));
        services.Configure<DatabaseConfigOptions>(opt => configuration.GetSection("Database").Bind(opt));
        services.Configure<JwtConfigOptions>(opt => securitySection.GetSection("Jwt").Bind(opt));
        services.AddScoped<IPasswordHasher,PasswordHasher>();
        services.AddScoped<IUserDomainRulesConfigProvider,UserDomainRulesConfigProvider>();
        services.AddScoped<IUserAuthenticator,UserAuthenticator>();
        services.InjectDatabaseService(configuration);
        services.InjectAuthenticationService(configuration);
        services.AddScoped<IUserRepository,UserRepository>();
        services.AddScoped<IUnitOfWork,UnitOfWork>();
        services.AddScoped<IUserUniquenessChecker,UserUniquenessChecker>();
        return services;
    }

    static IServiceCollection InjectDatabaseService(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>((sp,options) =>
        {
            var rawConnectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(rawConnectionString) || string.IsNullOrWhiteSpace(rawConnectionString))
                rawConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
            var csBuilder = new NpgsqlConnectionStringBuilder(rawConnectionString);
            var connectionString = csBuilder.ConnectionString;
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
    static IServiceCollection InjectAuthenticationService(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfigSection = configuration.GetSection("Security").GetSection("Jwt");
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfigSection.GetValue<string>("Issuer"),
                    ValidAudience = jwtConfigSection.GetValue<string>("Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfigSection.GetValue<string>("SecretKey") ?? ""))
                };

                options.Events = new JwtBearerEvents{
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["access_token"];
                        return Task.CompletedTask;
                    }
                };
            });
        return services;
    }
}