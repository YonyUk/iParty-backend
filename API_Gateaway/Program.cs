using ApiGateaway.Configuration;
using ApiGateaway.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RateLimitOptions>(
    opt => builder.Configuration.GetSection("RateLimitOptions").Bind(opt)
);

builder.Services.Configure<CorsConfigOptions>(
    opt => builder.Configuration.GetSection("CorsSettings").Bind(opt)
);

var corsPolicyName = "FrontendPolicy";

builder.Services.AddCorsConfiguration(corsPolicyName);
builder.Services.AddAuthenticationService(builder.Configuration);
builder.Services.AddAuthorizationService();
builder.Services.AddHealthChecks();
builder.Services.AddRateLimitService();
builder.Services.AddReverseProxyService(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(corsPolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapHealthChecks("/health");

app.MapGet("/ingo", () => Results.Ok(new
{
    GateAway = "Yarp",
    Status = "Running",
    Timestamp = DateTime.UtcNow
}));

app.MapReverseProxy();
app.Run();
