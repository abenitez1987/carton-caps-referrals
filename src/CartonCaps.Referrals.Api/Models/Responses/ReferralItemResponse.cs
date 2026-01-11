
using CartonCaps.Referrals.Api.Models.enums;

public class ReferralItemResponse
{
    public ReferralStatus Status { get; set; }
    public string? ReferralName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}