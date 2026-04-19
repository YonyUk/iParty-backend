using System.Text;
using System.Threading.RateLimiting;
using ApiGateaway.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ApiGateaway.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthenticationService(this IServiceCollection services, IConfiguration configuration)
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

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["access_token"];
                        return Task.CompletedTask;
                    }
                };
            });
        return services;
    }
    public static IServiceCollection AddAuthorizationService(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());
        });
        return services;
    }
    public static IServiceCollection AddRateLimitService(this IServiceCollection services)
    {

        services.AddRateLimiter(rateLimiterOptions =>
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var rOptions = scope.ServiceProvider.GetRequiredService<IOptions<RateLimitOptions>>().Value;
            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            rateLimiterOptions.AddFixedWindowLimiter("fixed", opt =>
            {
                opt.PermitLimit = rOptions.Limit;
                opt.Window = TimeSpan.FromMinutes(rOptions.TimeSpanLimit);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = rOptions.QueueLimit;
            });
        });
        return services;
    }
    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services,string policyName)
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        var corsOptions = scope.ServiceProvider.GetRequiredService<IOptions<CorsConfigOptions>>().Value;
        foreach(var org in corsOptions.AllowedOrigins)
            System.Console.WriteLine(org);
        services.AddCors(options =>
        {
            options.AddPolicy(policyName,policy =>
            {
                if (corsOptions.AllowedOrigins.Any())
                    policy.WithOrigins(corsOptions.AllowedOrigins);
                if (corsOptions.AllowedCredentials)
                    policy.AllowCredentials();
                if (corsOptions.AllowedHeader)
                    policy.AllowAnyHeader();
                if (corsOptions.AllowedMethod)
                    policy.AllowAnyMethod();
            });
        });
        return services;
    }
    public static IServiceCollection AddReverseProxyService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"));
        return services;
    }
}