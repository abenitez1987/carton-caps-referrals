
using CartonCaps.Referrals.Api.Models.Responses;

namespace CartonCaps.Referrals.Api.Services.Interfaces;
public interface IReferralsService
{
    Task<CreateReferralResponse> CreateReferralAsync(Guid userGuid, CreateReferralRequest request);
    Task<ValidateTrackingResponse> ValidateTrackingIdAsync(string trackingId);
    Task<ListReferralResponse> GetReferralsAsync(Guid userId);
}