using CartonCaps.Referrals.Api.Models;
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
}