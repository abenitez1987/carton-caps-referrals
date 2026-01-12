
using Xunit;

namespace CartonCaps.Referrals.IntegrationTests;
public abstract class IntegrationTestBase :IClassFixture<CustomWebApplicationFactory>
{
    protected readonly HttpClient _client;
    protected readonly CustomWebApplicationFactory _factory;

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
}