public class ListReferralResponse
{
    public string ReferralCode { get; set; } = string.Empty;
    public List<ReferralItemResponse> Data { get; set; } = new();
}