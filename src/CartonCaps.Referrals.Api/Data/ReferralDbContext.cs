using CartonCaps.Referrals.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CartonCaps.Referrals.Api.Data;
public class ReferralDbContext : DbContext
{
    public ReferralDbContext(DbContextOptions<ReferralDbContext> options)
        : base(options)
    {
    }

    public DbSet<Referral> Referrals { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ReferralCode).IsRequired().HasMaxLength(20);
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.ReferralCode).IsUnique();
        });

        modelBuilder.Entity<Referral>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.TrackingId).IsRequired();
            entity.Property(e => e.RefereeUserId);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.CompletedAt);
            entity.Property(e => e.ExpiresAt).IsRequired();

            entity.HasIndex(e => e.TrackingId).IsUnique();

            entity.HasOne(e => e.RefereeUser)
                  .WithMany(e => e.Referrals)
                  .HasForeignKey(e => e.RefereeUserId)
                  .OnDelete(DeleteBehavior.Cascade);

        });
    }
}