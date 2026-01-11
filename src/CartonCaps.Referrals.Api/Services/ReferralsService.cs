using CartonCaps.Referrals.Api.Data.Repositories;
using CartonCaps.Referrals.Api.Models.Responses;
using CartonCaps.Referrals.Api.Services.Interfaces;

namespace CartonCaps.Referrals.Api.Services;
public class ReferralsService: IReferralsService
{
    private readonly IReferralsRepository _referralsRepository;
    private readonly ITrackingGenerator _trackingGenerator;

    private readonly IDeepLinkGenerator _deepLinkGenerator;
    public ReferralsService(IReferralsRepository referralsRepository, ITrackingGenerator trackingGenerator, IDeepLinkGenerator deepLinkGenerator)
    {
        _referralsRepository = referralsRepository;
        _trackingGenerator = trackingGenerator;
        _deepLinkGenerator = deepLinkGenerator;
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

    public async Task<CreateReferralResponse> CreateReferralAsync(Guid userGuid, CreateReferralRequest request)
    {
        var trackingId = _trackingGenerator.Generate();

        var referral = await _referralsRepository.CreateReferralAsync(userGuid, trackingId);

        var shareUrl = _deepLinkGenerator.GenerateDeepLink(trackingId);

        return new CreateReferralResponse
        {
            TrackingId = trackingId,
            ShareUrl = shareUrl,
            CreatedAt = referral.CreatedAt,
            ExpiresAt = referral.ExpiresAt,
            ShareContent = new ShareContentResponse
            {
                Type = request?.Channel ?? "sms",
                Body = $"Join me on CartonCaps and get exclusive rewards! Use my referral link: {shareUrl}",
                Subject = request?.Channel == "email" ? "Join me on CartonCaps!" : null,
            }
        };
    }

    public async Task<ValidateTrackingResponse> ValidateTrackingIdAsync(string trackingId)
    {
        var referral = await _referralsRepository.GetByTrackingIdAsync(trackingId);

        if (referral == null)
        {
            return new ValidateTrackingResponse
            {
                Valid = false,
            };
        }

        return new ValidateTrackingResponse
        {
            Valid = referral.ExpiresAt > DateTime.UtcNow,
            ExpiresAt = referral.ExpiresAt,
            ReferralCode = referral.ReferralCode,
        };
    }
}