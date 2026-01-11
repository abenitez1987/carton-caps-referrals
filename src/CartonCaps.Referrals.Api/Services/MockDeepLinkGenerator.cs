using CartonCaps.Referrals.Api.Services.Interfaces;

public class MockDeepLinkGenerator : IDeepLinkGenerator
{
    public string GenerateDeepLink(string trackingId)
    {
        // Mock implementation for deep link generation
        return $"https://mockdeeplink.com/referral/{trackingId}";
    }
}