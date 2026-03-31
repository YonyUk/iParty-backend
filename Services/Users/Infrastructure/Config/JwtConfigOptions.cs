namespace Users.Infrastructure.Configuration;

public record JwtConfigOptions
{
    public string SecretKey { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public int ExpiresMinutes { get; init; } = 5;
}