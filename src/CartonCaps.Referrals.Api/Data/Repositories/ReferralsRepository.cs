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

    public async Task<List<Referral>> GetReferralsByUserIdAsync(Guid userId, string? status)
    {
        var query = _context.Referrals
            .Where(r => r.ReferrerUserId == userId)
            .Include(r => r.ReferrerUser)
            .AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(status) && status.ToUpper() != "ALL" && Enum.TryParse<ReferralStatus>(status, true, out var statusEnum))
        {
            query = query.Where(r => r.Status == statusEnum);
        }
        
        return await _context.Referrals
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
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
            Id = Guid.NewGuid(),
            ReferrerUserId = userGuid,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Status = ReferralStatus.Pending,
            TrackingId = trackingId,
            ReferralCode = user.ReferralCode,
            Channel = channel
        };

        _context.Referrals.Add(referral);

        try
        {
             await _context.SaveChangesAsync();

        } catch (Exception ex)
        {
            throw new Exception("An error occurred while creating the referral", ex);
        }
       
        return referral;
    }

    public Task<Referral?> GetByTrackingIdAsync(string trackingId)
    {
        return _context.Referrals
            .Include(r => r.ReferrerUser)
            .FirstOrDefaultAsync(r => r.TrackingId == trackingId);
    }
}