using System.Text.Json.Serialization;
using Users.API.Middlewares;
using Users.Application.DependencyInjection;
using Users.Infrastructure.DependencyInjection;

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
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseMiddleware<ApplicationExceptionHandlerMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();