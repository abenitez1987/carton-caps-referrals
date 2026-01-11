
namespace CartonCaps.Referrals.Api.Services;
public interface IReferralsService
{  
    Task<ListReferralResponse> GetReferralsAsync(Guid userId);
}