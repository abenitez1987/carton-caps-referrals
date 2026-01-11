using CartonCaps.Referrals.Api.Models;

namespace CartonCaps.Referrals.Api.Data.Repositories;
public interface IReferralsRepository
{
    Task<Referral> CreateReferralAsync(Guid userGuid);
    Task<List<Referral>> GetReferralsByUserIdAsync(Guid userId);
    Task<User?> GetUserByIdAsync(Guid userId);
}