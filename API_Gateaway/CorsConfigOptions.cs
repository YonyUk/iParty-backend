namespace ApiGateaway.Configuration;

public record CorsConfigOptions
{
    public string[] AllowedOrigins { get; init; } = new string[] { };
    public bool AllowedHeader { get; init; } = false;
    public bool AllowedMethod { get; init; } = false;
    public bool AllowedCredentials { get; init; } = false;
}