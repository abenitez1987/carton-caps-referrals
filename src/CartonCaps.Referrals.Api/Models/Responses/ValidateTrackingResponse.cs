namespace CartonCaps.Referrals.Api.Models.Responses;
public class ValidateTrackingResponse
{    
    public bool Valid { get; set; }
    public string? ReferrerCode { get; set; }
    public string? ReferrerName { get; set; }
    public string? Error { get; set; }
    public string? Message { get; set; }
}