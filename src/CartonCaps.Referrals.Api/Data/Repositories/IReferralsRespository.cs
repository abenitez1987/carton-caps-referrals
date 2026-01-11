using CartonCaps.Referrals.Api.Models;

namespace CartonCaps.Referrals.Api.Data.Repositories;
public interface IReferralsRepository
{
    Task<Referral> CreateReferralAsync(Guid userGuid, string trackingId, string channel);
    Task<List<Referral>> GetReferralsByUserIdAsync(Guid userId);
    Task<Referral?> GetByTrackingIdAsync(string trackingId);
    Task<User?> GetUserByIdAsync(Guid userId);
}