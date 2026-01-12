
using System.Text.Json.Serialization;
using CartonCaps.Referrals.Api.Models.enums;

public class ReferralItemResponse
{
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ReferralStatus Status { get; set; }   
    public string Channel { get; set; } = string.Empty;    
    public string? RefereeName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}