
using CartonCaps.Referrals.Api.Models.enums;

namespace CartonCaps.Referrals.Api.Models;
public class Referral 
{
    public Guid Id { get; set; }
    public string? Name { get; set; } = string.Empty;
    public string TrackingId { get; set; } = string.Empty;
    public Guid? RefereeUserId { get; set; }
    public ReferralStatus Status { get; set; }
    public User? RefereeUser { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

