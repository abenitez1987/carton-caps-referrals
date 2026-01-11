using CartonCaps.Referrals.Api.Models;
using CartonCaps.Referrals.Api.Models.enums;
using Microsoft.EntityFrameworkCore;

namespace CartonCaps.Referrals.Api.Data.Repositories;
public class ReferralsRepository : IReferralsRepository
{
    private readonly ReferralDbContext _context;

    public ReferralsRepository(ReferralDbContext context)
    {
        _context = context;
    }

    public async Task<List<Referral>> GetReferralsByUserIdAsync(Guid userId)
    {
        return await _context.Referrals
            .Where(r => r.RefereeUserId == userId)
            .Include(r => r.RefereeUser)
            .ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task<Referral> CreateReferralAsync(Guid userGuid, string trackingId, string channel)
    {
        var user = await _context.Users.FindAsync(userGuid);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        var referral = new Referral
        {
            RefereeUserId = userGuid,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            Status = ReferralStatus.Pending,
            TrackingId = trackingId,
            ReferralCode = user.ReferralCode,
            Channel = channel
        };

        _context.Referrals.Add(referral);

        try
        {
             await _context.SaveChangesAsync();

        } catch (DbUpdateException ex)
        {
            throw new Exception("An error occurred while creating the referral", ex);
        }
       
        return referral;
    }

    public Task<Referral?> GetByTrackingIdAsync(string trackingId)
    {
        return _context.Referrals
            .Include(r => r.RefereeUser)
            .FirstOrDefaultAsync(r => r.TrackingId == trackingId);
    }
}