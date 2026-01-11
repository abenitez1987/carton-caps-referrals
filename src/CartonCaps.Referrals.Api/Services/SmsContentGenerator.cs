namespace CartonCaps.Referrals.Api.Services;
public class SmsContentGenerator : IShareContentGenerator
{
    public string ChannelType => "SMS";
    public ShareContentResponse GenerateContent(string referralCode, string shareUrl)
    {
        var body = $"Hi! Join me in earning money for our school by using the Carton Caps app. " +
                   $"It's an easy way to make a difference. " +
                   $"Use the link below to download the Carton Caps app: {shareUrl} " +
                   $"(referral_code={referralCode})";

        return new ShareContentResponse
        {
            Type = ChannelType,
            Body = body
        };
    }
}