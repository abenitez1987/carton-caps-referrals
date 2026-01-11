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
            Id = Guid.NewGuid(),
            Email = "andres@test.com",
            FirstName = "Andres",
            LastName = "Guzman",
            ReferralCode = "ANDRES123",
            CreatedAt = DateTime.UtcNow.AddMonths(-2)
        };

        var user2 = new User
        {
            Id = Guid.NewGuid(),
            Email = "carlos@test.com",
            FirstName = "Carlos",
            LastName = "Lopez",
            ReferralCode = "CARLOS123",
            CreatedAt = DateTime.UtcNow.AddMonths(-1)
        };

        var user3 = new User
        {
            Id = Guid.NewGuid(),
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
            TrackingId = "track_completed_001",
            RefereeUserId = user1.Id,
            Status = ReferralStatus.Completed,
            RefereeUser = user1,
        };

        var referral2 = new Referral
        {
            Id = Guid.NewGuid(),
            TrackingId = "track_pending_001",
            RefereeUserId = user1.Id,
            Status = ReferralStatus.Pending,
            RefereeUser = user1,
        };

        var referral3 = new Referral
        {
            Id = Guid.NewGuid(),
            TrackingId = "track_completed_002",
            RefereeUserId = user2.Id,
            Status = ReferralStatus.Completed,
            RefereeUser = user2,
        };

        var referral4 = new Referral
        {
            Id = Guid.NewGuid(),
            TrackingId = "track_pending_002",
            RefereeUserId = user2.Id,
            Status = ReferralStatus.Pending,
            RefereeUser = user2,
        };

        var referral5 = new Referral
        {
            Id = Guid.NewGuid(),
            TrackingId = "track_expired_001",
            RefereeUserId = user2.Id,
            Status = ReferralStatus.Expired,
            RefereeUser = user2,
        };
        context.Referrals.AddRange(referral1, referral2, referral3, referral4, referral5);
        context.SaveChanges();
    }
}