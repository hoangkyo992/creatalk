namespace Common.Api;

public enum RateLimitPolicy
{
    IdentityFixed,
    IdentityConcurrency,
    IdentitySliding,
    IdentityTokenBucket,

    IpAddressFixed,
    IpAddressConcurrency,
    IpAddressSliding,
    IpAddressTokenBucket
}