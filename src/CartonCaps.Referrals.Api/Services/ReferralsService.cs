using CartonCaps.Referrals.Api.Data.Repositories;
using CartonCaps.Referrals.Api.Services;

public class ReferralsService: IReferralsService
{
    private readonly IReferralsRepository _referralsRepository;
    public ReferralsService(IReferralsRepository referralsRepository)
    {
        _referralsRepository = referralsRepository;
    }
    public async Task<ListReferralResponse> GetReferralsAsync(Guid userId)
    {
        var referrals = await _referralsRepository.GetReferralsByUserIdAsync(userId);

        return new ListReferralResponse
        {
            ReferralCode = referrals.FirstOrDefault()?.RefereeUser?.ReferralCode ?? string.Empty,
            Data = referrals.Select(r => new ReferralItemResponse
            {
                Status = r.Status,
                ReferralName = r.Name != null ? $"{r.Name}" : string.Empty,
                CreatedAt = r.CreatedAt,
                CompletedAt = r.CompletedAt,
                ExpiresAt = r.ExpiresAt
            }).ToList()
        };
    }
}