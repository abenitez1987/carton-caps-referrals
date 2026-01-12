
using CartonCaps.Referrals.Api.Data;
using Microsoft.Extensions.DependencyInjection;
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

    protected void SetAuthHeader(string userId)
    {
        _client.DefaultRequestHeaders.Remove("X-User_Id");
        _client.DefaultRequestHeaders.Add("X-User-Id", userId);
    }

    protected void ClearAuthHeader()
    {
        _client.DefaultRequestHeaders.Remove("X-User-Id");
    }

    protected ReferralDbContext GetDbContext()
    {
        var scope = _factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ReferralDbContext>();
    }
}