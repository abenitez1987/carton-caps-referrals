
namespace CartonCaps.Referrals.Api.Services.Interfaces;
public interface IShareContentService
{
    Dictionary<string,object> GeneateAllContent(string referralCode, string shareUrl);
}