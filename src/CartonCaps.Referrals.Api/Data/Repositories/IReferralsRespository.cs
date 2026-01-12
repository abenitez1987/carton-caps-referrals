using CartonCaps.Referrals.Api.Models;

namespace CartonCaps.Referrals.Api.Data.Repositories;
public interface IReferralsRepository
{
    Task<Referral?> CreateReferralAsync(Guid userGuid, string trackingId, string channel, string referrerCode);
    Task<List<Referral>> GetReferralsByUserIdAsync(Guid userId, string? status);
    Task<Referral?> GetByTrackingIdAsync(string trackingId);
}