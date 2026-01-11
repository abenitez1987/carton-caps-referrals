
namespace CartonCaps.Referrals.Api.Services.Interfaces;
public interface IShareContentFactory
{
    IShareContentGenerator GetGenerator(string channelType);
}