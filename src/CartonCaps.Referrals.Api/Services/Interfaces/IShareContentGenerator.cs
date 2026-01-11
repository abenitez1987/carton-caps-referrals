public interface IShareContentGenerator
{
    string ChannelType { get; }
    ShareContentResponse GenerateContent(string referralCode, string shareUrl);
}