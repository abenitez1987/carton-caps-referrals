using CartonCaps.Referrals.Api.Models;
using CartonCaps.Referrals.Api.Models.enums;

namespace CartonCaps.Referrals.Api.Data;
public static class DatabaseSeeder
{
    public static void Seed(ReferralDbContext context)
    {
        context.Referrals.RemoveRange(context.Referrals);
        context.Users.RemoveRange(context.Users);
        context.SaveChanges();

        var user1 = new User
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Email = "andres@test.com",
            FirstName = "Andres",
            LastName = "Benitez",
            ReferralCode = "ANDRES123",
            CreatedAt = DateTime.UtcNow.AddMonths(-2)
        };

        var user2 = new User
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Email = "carlos@test.com",
            FirstName = "Carlos",
            LastName = "Lopez",
            ReferralCode = "CARLOS123",
            CreatedAt = DateTime.UtcNow.AddMonths(-1)
        };

        var user3 = new User
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Email = "mafe@test.com",
            FirstName = "Mafe",
            LastName = "Rodriguez",
            ReferralCode = "MAFE123",
            CreatedAt = DateTime.UtcNow.AddMonths(-3)
        };

        context.Users.AddRange(user1, user2, user3);

        var referral1 = new Referral
        {
            Id = Guid.NewGuid(),
            RefereeName = "Lorenzo",
            Channel = "email",
            TrackingId = "track_completed_001",
            ReferrerUserId = user1.Id,
            Status = ReferralStatus.Completed,
            ReferrerUser = user1,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            CompletedAt = DateTime.UtcNow.AddDays(-5),
            ExpiresAt = DateTime.UtcNow.AddDays(20)
        };

        var referral2 = new Referral
        {
            Id = Guid.NewGuid(),
            RefereeName = null,
            Channel = "sms",
            TrackingId = "track_pending_001",
            ReferrerUserId = user1.Id,
            Status = ReferralStatus.Pending,
            ReferrerUser = user1,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            ExpiresAt = DateTime.UtcNow.AddDays(28)
        };

        var referral3 = new Referral
        {
            Id = Guid.NewGuid(),
            RefereeName = "Joaquin",
            Channel = "sms",
            TrackingId = "track_completed_002",
            ReferrerUserId = user1.Id,
            Status = ReferralStatus.Completed,
            ReferrerUser = user1,
            CreatedAt = DateTime.UtcNow.AddDays(-15),
            CompletedAt = DateTime.UtcNow.AddDays(-10),
            ExpiresAt = DateTime.UtcNow.AddDays(15)
        };

        var referral4 = new Referral
        {
            Id = Guid.NewGuid(),
            RefereeName = null,
            Channel = "email",
            TrackingId = "track_pending_002",
            ReferrerUserId = user2.Id,
            Status = ReferralStatus.Pending,
            ReferrerUser = user2,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            ExpiresAt = DateTime.UtcNow.AddDays(29)
        };

        var referral5 = new Referral
        {
            Id = Guid.NewGuid(),
            RefereeName = null,
            Channel = "sms",
            TrackingId = "track_expired_001",
            ReferrerUserId = user2.Id,
            Status = ReferralStatus.Expired,
            ReferrerUser = user2,
            CreatedAt = DateTime.UtcNow.AddDays(-40),
            ExpiresAt = DateTime.UtcNow.AddDays(-10)
        };
        context.Referrals.AddRange(referral1, referral2, referral3, referral4, referral5);
        context.SaveChanges();
    }
}