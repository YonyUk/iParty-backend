using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Users.API.Converters;
using Users.API.Middlewares;
using Users.Application.DependencyInjection;
using Users.Infrastructure.DependencyInjection;
using Users.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddPersistenceService(builder.Configuration);
builder.Services.AddTransient<ApplicationExceptionHandlerMiddleware>();

builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "Users api";
    config.Version = "v1";
});
// adds the global prefix
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new GlobalRoutePrefixConvention(builder.Configuration));
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new UserRoleJsonConverter());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (dbContext.Database.GetPendingMigrations().Any())
    {
        dbContext.Database.Migrate();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseOpenApi();
    app.UseSwaggerUi();
}
app.UseMiddleware<ApplicationExceptionHandlerMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();

public partial class Program { }