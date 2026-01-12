using System.Net;
using CartonCaps.Referrals.IntegrationTests;
using FluentAssertions;
using Xunit;

public class HealthTests: IntegrationTestBase
{
    public HealthTests(CustomWebApplicationFactory factory): base(factory)
    {
        
    }

    [Fact]
    public async Task Get_Referrals_Unauthorized_When_No_Token()
    {
        var response = await _client.GetAsync("/api/v1/referrals");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
}