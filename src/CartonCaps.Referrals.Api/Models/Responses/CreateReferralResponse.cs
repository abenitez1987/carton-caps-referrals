public class CreateReferralResponse 
{
    public string TrackingId { get; set; } = string.Empty;
    public string ShareUrl { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public ShareContentResponse ShareContent { get; set; } = new ShareContentResponse();
}