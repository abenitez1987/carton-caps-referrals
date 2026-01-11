
using CartonCaps.Referrals.Api.Models.enums;

namespace CartonCaps.Referrals.Api.Models;
public class Referral 
{
    public Guid Id { get; set; }
    public string TrackingId { get; set; } = string.Empty;
    public string ReferralCode { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;

    public Guid? ReferrerUserId { get; set; }
    public User? ReferrerUser { get; set; }

    public Guid? RefereeUserId { get; set; }
    public User? RefereeUser { get; set; }
    public string? RefereeName { get; set; }

    public ReferralStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}

