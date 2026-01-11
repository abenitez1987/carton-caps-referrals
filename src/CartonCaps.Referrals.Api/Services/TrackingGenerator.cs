using CartonCaps.Referrals.Api.Services.Interfaces;

namespace CartonCaps.Referrals.Api.Services;
public class TrackingGenerator : ITrackingGenerator
{
    public string Generate()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var random = Guid.NewGuid().ToString("N").Substring(0, 8);

        return $"tkr_{timestamp}_{random}";
    }
}