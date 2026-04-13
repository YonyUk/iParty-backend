using ApiGateaway.Configuration;
using ApiGateaway.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RateLimitOptions>(
    opt => builder.Configuration.GetSection("RateLimitOptions").Bind(opt)
);

builder.Services.AddAuthenticationService(builder.Configuration);
builder.Services.AddAuthorizationService();
builder.Services.AddHealthChecks();
builder.Services.AddRateLimitService();
builder.Services.AddReverseProxyService(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapHealthChecks("/health");

app.MapGet("/info", () => Results.Ok(new
{
    GateAway = "Yarp",
    Status = "Running",
    Timestamp = DateTime.UtcNow
}));

app.MapReverseProxy();
app.Run();
