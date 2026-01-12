using System.Net;
using System.Net.Http.Json;
using CartonCaps.Referrals.Api.Models;
using CartonCaps.Referrals.Api.Models.enums;
using CartonCaps.Referrals.Api.Models.Responses;
using CartonCaps.Referrals.IntegrationTests;
using FluentAssertions;
using Xunit;

public class ReferralJourneyTests : IntegrationTestBase
{
    public ReferralJourneyTests(CustomWebApplicationFactory factory) : base(factory)
    {
        
    }

    [Fact]
    public async Task Journey_CreateReferral_ValidateTracking_ListReferrals_Completed()
    {
        // User A emulate login and create a new referral
        var userA = "11111111-1111-1111-1111-111111111111";
        SetAuthHeader(userA);

        // ReferrerCode is coming from the mobile/web app client. that was part of the initial assumption.
        var createRequest = new CreateReferralRequest { Channel = "sms", ReferrerCode="ANDRES123" };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/referrals", createRequest);

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var referral = await createResponse.Content
            .ReadFromJsonAsync<CreateReferralResponse>();

        referral.Should().NotBeNull();
        referral!.TrackingId.Should().MatchRegex(@"^trk_\d{10}_[a-f0-9]{8}$");
        referral.ShareUrl.Should().Contain(referral.TrackingId);
        referral.ExpiresAt.Should().BeAfter(DateTime.UtcNow);

        referral.ShareContent.Should().NotBeNull();
        var content = referral.ShareContent as IDictionary<string, object>;
        content.Should().ContainKey("sms");
        content.Should().ContainKey("email");

        // User B simulate open the link and open the app for the first time
        ClearAuthHeader();

        var validateResponse = await _client.GetAsync(
            $"/api/v1/referrals/{referral.TrackingId}");
        
        validateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var validation = await validateResponse.Content
            .ReadFromJsonAsync<ValidateTrackingResponse>();

        
        validation.Should().NotBeNull();
        validation!.Valid.Should().BeTrue();
        validation.ReferrerCode.Should().Be("ANDRES123");
        validation.ReferrerName.Should().Contain("Andres");
        

        SetAuthHeader(userA);
        
        var listResponse = await _client.GetAsync("/api/v1/referrals");
        var list = await listResponse.Content.ReadFromJsonAsync<ListReferralResponse>();

        list.Should().NotBeNull();
        list.Data.Any(x => x.Status == ReferralStatus.Pending && x.RefereeName is null);

        // Emulate that UserB creates a new profile as referred
        ClearAuthHeader();
        var userB = await CreateUserRefferredInDb(validation.ReferrerCode);

        SetAuthHeader(userB);
        var emptyList = await _client.GetAsync("/api/v1/referrals");
        var emptyReferralResponse = await emptyList.Content.ReadFromJsonAsync<ListReferralResponse>();

        emptyReferralResponse.Should().NotBeNull();
    }

    [Fact]
    public async Task Journey_MultipleValidations_BeforeRedemption_AreAllowed()
    {
         var userA = "11111111-1111-1111-1111-111111111111";
        SetAuthHeader(userA);

        var createRequest = new CreateReferralRequest { Channel = "sms" };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/referrals", createRequest);

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var referral = await createResponse.Content
            .ReadFromJsonAsync<CreateReferralResponse>();

        ClearAuthHeader();

        for (int i = 0; i < 5; i++)
        {
             var response = await _client.GetAsync($"/api/v1/referrals/{referral!.TrackingId}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var validation = await response.Content
                .ReadFromJsonAsync<ValidateTrackingResponse>();
            
            validation!.Valid.Should().BeTrue();
            validation.ReferrerCode.Should().Be("ANDRES123");
        }

        // Confirm that the referral keeps PENDING
        SetAuthHeader(userA);
        
        var listResponse = await _client.GetAsync("/api/v1/referrals");
        var list = await listResponse.Content.ReadFromJsonAsync<ListReferralResponse>();

        list.Should().NotBeNull();
        list.Data.Any(x => x.Status == ReferralStatus.Pending && x.RefereeName is null);
    }

    [Fact]
    public async Task Journey_ExpiredTracking_ReturnInvalid()
    {
        var expiredTrackingId = await CreateExpiredReferralInDb();
        ClearAuthHeader();
        
        var response = await _client.GetAsync($"/api/v1/referrals/{expiredTrackingId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var validation = await response.Content
            .ReadFromJsonAsync<ValidateTrackingResponse>();

        validation.Should().NotBeNull();
        validation!.Valid.Should().BeFalse();
        validation.Error.Should().Be("TRACKING_ID__EXPIRED");
        validation.Message.Should().Be("Tracking Id has expired.");
    }

    [Fact]
    public async Task Journey_NonExistentTracking_ReturnsInvalid()
    {
         ClearAuthHeader();
        
        var nonExistentTracking = "trk_1768240747_a78a7496";
        var response = await _client.GetAsync($"/api/v1/referrals/{nonExistentTracking}");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var validation = await response.Content
            .ReadFromJsonAsync<ValidateTrackingResponse>();

        validation.Should().NotBeNull();
        validation!.Valid.Should().BeFalse();
        validation.Error.Should().Be("TRACKING_ID_NOT_FOUND");
        validation.Message.Should().Be("The tracking ID does not exist.");
    }

    private async Task<string> CreateExpiredReferralInDb()
    {
        var db = GetDbContext();

        var expiredReferral = new Referral
        {
            Id = Guid.NewGuid(),
            TrackingId = $"trk_1768240747_a78a7496",
            ReferrerCode = "MARIO123",
            Channel = "sms",
            ReferrerUserId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Status = ReferralStatus.Expired,
            CreatedAt = DateTime.UtcNow.AddDays(-15),
            ExpiresAt = DateTime.UtcNow.AddDays(-8)
        };

        db.Referrals.Add(expiredReferral);
        await db.SaveChangesAsync();

        return expiredReferral.TrackingId;
    }

    private async Task<string> CreateUserRefferredInDb(string referralCode)
    {
        var db = GetDbContext();

        var referredUser = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Pablo",
            LastName = "Flores",
            ReferredByCode = referralCode,
            Email = "pablo@test.com",
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(referredUser);
        await db.SaveChangesAsync();

        return referredUser.Id.ToString();
    } 
}