using CartonCaps.Referrals.Api.Services;
using Microsoft.AspNetCore.Authorization;
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

        var referral = await _referralsService.GetReferralsAsync(userGuid);
        return Ok(referral);
    }

    [HttpPost()]
    [Authorize]
    public async Task<ActionResult<CreateReferralResponse>> Post([FromBody] CreateReferralRequest request)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (userId == null || !Guid.TryParse(userId, out Guid userGuid))
        {   
            _logger.LogWarning("Unauthorized access attempt to Create Referral");
            return Unauthorized();
        }

        var response = await _referralsService.CreateReferralAsync(userGuid, request);

        return Ok(response);
    }
}