using CartonCaps.Referrals.Api.Services.Interfaces;

public class ShareContentService : IShareContentService
{
    private readonly ILogger<ShareContentService> _logger;
    private readonly IEnumerable<IShareContentGenerator> _generators;
    public ShareContentService(ILogger<ShareContentService> logger, IEnumerable<IShareContentGenerator> generators)
    {
        _generators = generators;
        _logger = logger;
    }

    public Dictionary<string,object> GeneateAllContent(string referralCode, string shareUrl)
    {
        var allContent = new Dictionary<string, object>();
        foreach (var generator in _generators)
        {
            try
            {
                var content = generator.GenerateContent(referralCode, shareUrl);
                allContent.Add(generator.ChannelType,content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating content for channel {ChannelType}", generator.ChannelType);
            }
        }
        return allContent;
    }
}