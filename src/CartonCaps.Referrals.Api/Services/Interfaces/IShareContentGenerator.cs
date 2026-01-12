public interface IShareContentGenerator
{
    string ChannelType { get; }
    object GenerateContent(string referralCode, string shareUrl);
}