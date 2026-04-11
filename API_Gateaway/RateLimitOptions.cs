namespace ApiGateaway.Configuration;

public record RateLimitOptions
{
    public int Limit { get; init; } = 100;
    public int TimeSpanLimit { get; init; } = 1;
    public int QueueLimit { get; init; } = 10;
}