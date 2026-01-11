namespace CartonCaps.Referrals.Api.Services;
public class EmailContentGenerator : IShareContentGenerator
{
    public string ChannelType => "EMAIL";
    public ShareContentResponse GenerateContent(string referralCode, string shareUrl)
    {
        var subject = "You're invited to try the Carton Caps app!";
        var body = "Hey!\n\n" +
                   "Join me in earning cash for our school by using the Carton Caps app. " +
                   "It's an easy way to make a difference. " +
                   "All you have to do is buy Carton Caps participating products (like Cheerios!) and scan your grocery receipt. " +
                   "Carton Caps are worth $.10 each and they add up fast! " +
                   "Twice a year, our school receives a check to help pay for whatever we need - equipment, supplies, or experiences the kids love!\n\n" +
                   $"Download the Carton Caps app here: {shareUrl}\n" +
                   $"(referral_code={referralCode})";

        return new ShareContentResponse
        {
            Type = ChannelType,
            Subject = subject,
            Body = body
        };
    }
}