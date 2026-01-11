
namespace CartonCaps.Referrals.Api.Services.Interfaces;
public interface IDeepLinkGenerator
{
    string GenerateDeepLink(string trackingId);
}