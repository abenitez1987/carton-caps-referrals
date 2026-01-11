namespace CartonCaps.Referrals.Api.Models.Responses;
public class ValidateTrackingResponse
{    
    public string ReferralCode { get; set; } = string.Empty;
    public bool Valid { get; set; }
    public DateTime ExpiresAt { get; set; }
}