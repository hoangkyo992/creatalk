namespace Common.Api.Configurations;

public class RateLimitsOptions : RateLimitPolicyOptions
{
    public const string RateLimits = "RateLimits";

    public Dictionary<string, RateLimitPolicyOptions> Policies { get; init; } = new();
}

public class RateLimitPolicyOptions
{
    public int PermitLimit { get; set; } = 100;
    public int Window { get; set; } = 6;
    public int ReplenishmentPeriod { get; set; } = 2;
    public int QueueLimit { get; set; } = 2;
    public int SegmentsPerWindow { get; set; } = 8;
    public int TokenLimit { get; set; } = 20;
    public int TokensPerPeriod { get; set; } = 4;
    public bool AutoReplenishment { get; set; } = false;
}