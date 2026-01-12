using CartonCaps.Referrals.Api.Models.Responses;
using CartonCaps.Referrals.Api.Services;
using CartonCaps.Referrals.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CartonCaps.Referrals.Api.Controllers;
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

    /// <summary>
    /// Get Referrals for the authenticated user
    /// </summary>
    /// <param name="status">
    /// Optional filter by status: PENDING, COMPLETED, EXPIRED, ALL.
    /// If not provided, returns all referrals.
    /// </param>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ListReferralResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ListReferralResponse>> GetReferrals([FromQuery] string? status = null)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (userId == null || !Guid.TryParse(userId, out Guid userGuid))
        {   
            _logger.LogWarning("Unauthorized access attempt to Get Referrals");
            return Unauthorized();
        }

        _logger.LogInformation("Fetching referrals for user: {UserId}", userGuid);
        var referral = await _referralsService.GetReferralsAsync(userGuid, status);
        return Ok(referral);
    }

    /// <summary>
    ///  Create a new Referral for the authenticated user
    /// </summary>
    /// <param name="request"></param>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(CreateReferralResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CreateReferralResponse>> CreateReferral([FromBody] CreateReferralRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = User.FindFirst("userId")?.Value;
        if (userId == null || !Guid.TryParse(userId, out Guid userGuid))
        {   
            _logger.LogWarning("Unauthorized access attempt to Create Referral");
            return Unauthorized();
        }

        _logger.LogInformation("Creating referral for user: {UserId}", userGuid);
        var response = await _referralsService.CreateReferralAsync(userGuid, request);

        return CreatedAtAction(
            nameof(CreateReferral), 
            new { trackingId = response.TrackingId }, 
            response
        );
    }

    /// <summary>
    /// Validate a tracking ID
    /// </summary>
    /// <param name="trackingId"></param>
    [HttpGet("{trackingId}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ValidateTrackingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ValidateTrackingResponse>> ValidateTracking(string trackingId)
    {
        if (string.IsNullOrWhiteSpace(trackingId))
        {
            _logger.LogWarning("Tracking ID is null or empty in ValidateTrackingIdAsync");
            return BadRequest("Tracking ID is required");
        }

        _logger.LogInformation("Validating tracking ID: {TrackingId}", trackingId);
        var response = await _referralsService.ValidateTrackingIdAsync(trackingId);

        return Ok(response);
    }
}