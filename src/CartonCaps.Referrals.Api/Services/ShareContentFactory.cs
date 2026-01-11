using CartonCaps.Referrals.Api.Services.Interfaces;

public class ShareContentFactory: IShareContentFactory
{
    private readonly ILogger<ShareContentFactory> _logger;
    private readonly IEnumerable<IShareContentGenerator> _generators;
    public ShareContentFactory(ILogger<ShareContentFactory> logger, IEnumerable<IShareContentGenerator> generators)
    {
        _generators = generators;
        _logger = logger;
    }
    public IShareContentGenerator GetGenerator(string channelType)
    {
        var generator = _generators.FirstOrDefault(g => g.ChannelType.Equals(channelType, StringComparison.OrdinalIgnoreCase));

        if (generator == null)
        {
            _logger.LogError("No content generator found for channel type: {ChannelType}", channelType);
            throw new NotSupportedException($"Channel type '{channelType}' is not supported.");
        }

        return generator;
    }
}