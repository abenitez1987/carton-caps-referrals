using CartonCaps.Referrals.Api.Models.Responses;
using CartonCaps.Referrals.Api.Services;
using CartonCaps.Referrals.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
public class ReferralsController : ControllerBase
{
    private readonly ILogger<ReferralsController> _logger;
    private readonly IReferralsService _referralsService;
    public ReferralsController(ILogger<ReferralsController> logger, IReferralsService referralsService)
    {
        _logger = logger;
        _referralsService = referralsService;
    }


    [HttpGet()]
    [Authorize]
    public async Task<ActionResult<ListReferralResponse>> Get()
    {
        var userId = User.FindFirst("userId")?.Value;
        if (userId == null || !Guid.TryParse(userId, out Guid userGuid))
        {   
            _logger.LogWarning("Unauthorized access attempt to Get Referrals");
            return Unauthorized();
        }

        _logger.LogInformation("Fetching referrals for user: {UserId}", userGuid);
        var referral = await _referralsService.GetReferralsAsync(userGuid);
        return Ok(referral);
    }

    [HttpPost("Share")]
    [Authorize]
    public async Task<ActionResult<CreateReferralResponse>> Post([FromBody] CreateReferralRequest request)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (userId == null || !Guid.TryParse(userId, out Guid userGuid))
        {   
            _logger.LogWarning("Unauthorized access attempt to Create Referral");
            return Unauthorized();
        }

        _logger.LogInformation("Creating referral for user: {UserId}", userGuid);
        var response = await _referralsService.CreateReferralAsync(userGuid, request);

        return Ok(response);
    }

    [HttpGet("Tracking")]
    [AllowAnonymous]
    public async Task<ActionResult<ValidateTrackingResponse>> ValidateTrackingIdAsync(string trackingId)
    {
        if (string.IsNullOrEmpty(trackingId))
        {
            _logger.LogWarning("Tracking ID is null or empty in ValidateTrackingIdAsync");
            return BadRequest("Tracking ID is required");
        }

        _logger.LogInformation("Validating tracking ID: {TrackingId}", trackingId);
        var response = await _referralsService.ValidateTrackingIdAsync(trackingId);

        return Ok(response);
    }


}