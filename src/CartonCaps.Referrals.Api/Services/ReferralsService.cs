using CartonCaps.Referrals.Api.Data.Repositories;
using CartonCaps.Referrals.Api.Models.Responses;
using CartonCaps.Referrals.Api.Services.Interfaces;

namespace CartonCaps.Referrals.Api.Services;
public class ReferralsService: IReferralsService
{
    private readonly IReferralsRepository _referralsRepository;
    private readonly ITrackingGenerator _trackingGenerator;
    private readonly IShareContentFactory _shareContentFactory;
    private readonly IDeepLinkGenerator _deepLinkGenerator;
    public ReferralsService(IReferralsRepository referralsRepository, ITrackingGenerator trackingGenerator, IDeepLinkGenerator deepLinkGenerator, IShareContentFactory shareContentFactory)
    {
        _referralsRepository = referralsRepository;
        _trackingGenerator = trackingGenerator;
        _deepLinkGenerator = deepLinkGenerator;
        _shareContentFactory = shareContentFactory;
    }
    public async Task<ListReferralResponse> GetReferralsAsync(Guid userId)
    {
        var referrals = await _referralsRepository.GetReferralsByUserIdAsync(userId);

        return new ListReferralResponse
        {
            Data = referrals.Select(r => new ReferralItemResponse
            {
                Status = r.Status,
                RefereeName = r.RefereeName,
                Channel = r.Channel,
                CreatedAt = r.CreatedAt,
                CompletedAt = r.CompletedAt,
                ExpiresAt = r.ExpiresAt
            }).ToList()
        };
    }

    public async Task<CreateReferralResponse> CreateReferralAsync(Guid userGuid, CreateReferralRequest request)
    {
        var trackingId = _trackingGenerator.Generate();
        var referral = await _referralsRepository.CreateReferralAsync(userGuid, trackingId, request.Channel);

        var shareUrl = _deepLinkGenerator.GenerateDeepLink(trackingId);
        var shareGenerator = _shareContentFactory.GetGenerator(request.Channel);

        var content = shareGenerator.GenerateContent(referral.ReferralCode, shareUrl);

        return new CreateReferralResponse
        {
            TrackingId = trackingId,
            ShareUrl = shareUrl,
            CreatedAt = referral.CreatedAt,
            ExpiresAt = referral.ExpiresAt,
            ShareContent = content
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

        if (referral.ExpiresAt <= DateTime.UtcNow)
        {
            return new ValidateTrackingResponse
            {
                Valid = false,
                Error = "Referral Expired",
                Message = "The referral link has expired."
            };
        }

        return new ValidateTrackingResponse
        {
            Valid = referral.ExpiresAt > DateTime.UtcNow,
            ReferralCode = referral.ReferralCode,
        };
    }
}