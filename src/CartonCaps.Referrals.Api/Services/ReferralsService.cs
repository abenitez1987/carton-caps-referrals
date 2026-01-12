using System.Text.RegularExpressions;
using CartonCaps.Referrals.Api.Data.Repositories;
using CartonCaps.Referrals.Api.Models.enums;
using CartonCaps.Referrals.Api.Models.Responses;
using CartonCaps.Referrals.Api.Services.Interfaces;

namespace CartonCaps.Referrals.Api.Services;
public class ReferralsService: IReferralsService
{
    private readonly ILogger<ReferralsService> _logger;
    private readonly IReferralsRepository _referralsRepository;
    private readonly ITrackingGenerator _trackingGenerator;
    private readonly IShareContentService _shareContentService;
    private readonly IDeepLinkGenerator _deepLinkGenerator;
     private static readonly Regex TrackingIdRegex = new(
        @"^trk_\d{10}_[a-f0-9]{8}$",
        RegexOptions.Compiled);
    public ReferralsService(IReferralsRepository referralsRepository, ITrackingGenerator trackingGenerator, IDeepLinkGenerator deepLinkGenerator, IShareContentService shareContentFactory, ILogger<ReferralsService> logger)
    {
        _referralsRepository = referralsRepository;
        _trackingGenerator = trackingGenerator;
        _deepLinkGenerator = deepLinkGenerator;
        _shareContentService = shareContentFactory;
        _logger = logger;
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
        var allContent = _shareContentService.GeneateAllContent(referral.ReferralCode, shareUrl);

        return new CreateReferralResponse
        {
            TrackingId = trackingId,
            ShareUrl = shareUrl,
            CreatedAt = referral.CreatedAt,
            ExpiresAt = referral.ExpiresAt,
            ShareContent = allContent
        };
    }

    public async Task<ValidateTrackingResponse> ValidateTrackingIdAsync(string trackingId)
    {
        if (!IsValidTrackingIdFormat(trackingId))
        {
            _logger.LogWarning("Invalid tracking ID format: {TrackingId}", trackingId);
            return new ValidateTrackingResponse
            {
                Valid = false,
                Error = "INVALID_FORMAT",
                Message = "The tracking ID format is invalid."
            };
        }

        var referral = await _referralsRepository.GetByTrackingIdAsync(trackingId);

        if (referral == null)
        {
            _logger.LogWarning("Tracking ID not found: {TrackingId}", trackingId);
            return new ValidateTrackingResponse
            {
                Valid = false,
                Error = "TRACKING_ID_NOT_FOUND",
                Message = "The tracking ID does not exist."
            };
        }

        if (referral.ExpiresAt <= DateTime.UtcNow || referral.Status == ReferralStatus.Expired)
        {
            _logger.LogInformation("Tracking ID expired: {TrackingId}", trackingId);
            return new ValidateTrackingResponse
            {
                Valid = false,
                Error = "TRACKING_ID__EXPIRED",
                Message = "Tracking Id has expired."
            };
        }

        _logger.LogInformation("Tracking ID is valid: {TrackingId}", trackingId);

        return new ValidateTrackingResponse
        {
            Valid = true,
            ReferralCode = referral.ReferralCode,
            ReferrerName = referral.ReferrerUser?.FirstName ?? string.Empty
        };
    }

    private bool IsValidTrackingIdFormat(string trackingId)
    {
        return !string.IsNullOrWhiteSpace(trackingId) && TrackingIdRegex.IsMatch(trackingId);
    }
}