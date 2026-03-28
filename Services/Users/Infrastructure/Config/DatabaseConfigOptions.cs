namespace Users.Infrastructure.Configuration;

public record DatabaseConfigOptions
{
    public string DefaultConnection { get; init; } = string.Empty;
    public string Schema { get; init; } = "public";
    public int CommandTimeoutSeconds { get; init; } = 30;
    public bool EnableSensitiveDataLogging { get; init; } = false;
}