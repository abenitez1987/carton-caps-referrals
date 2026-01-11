using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class ReferralsController : ControllerBase
{
    
    [HttpGet("test-auth")]
    [Authorize]
    public IActionResult TestAuth()
    {
        var userId = User.FindFirst("userId")?.Value;
        return Ok(new { UserId = userId });
    }
}