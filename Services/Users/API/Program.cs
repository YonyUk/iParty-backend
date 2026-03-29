using Users.Application.DependencyInjection;
using Users.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddPersistenceService(builder.Configuration);

builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "Users api";
    config.Version = "v1";
});
// adds the global prefix
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new GlobalRoutePrefixConvention(builder.Configuration));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();