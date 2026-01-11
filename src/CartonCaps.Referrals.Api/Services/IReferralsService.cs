
namespace CartonCaps.Referrals.Api.Services;
public interface IReferralsService
{
    Task<CreateReferralResponse> CreateReferralAsync(Guid userGuid, CreateReferralRequest request);
    Task<ListReferralResponse> GetReferralsAsync(Guid userId);
}