using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Madayn.Web.Models;

namespace Madayn.Web.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<ProfileImageHistory> ProfileImageHistory => Set<ProfileImageHistory>();
    public DbSet<UserActivity> UserActivities => Set<UserActivity>();
    public DbSet<MadaynRegion> MadaynRegions => Set<MadaynRegion>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserProfile>()
            .Property(p => p.FullName)
            .HasComputedColumnSql("([FirstName] + ' ' + [LastName])", stored: true);

        builder.Entity<UserProfile>()
            .HasOne(p => p.Region)
            .WithMany()
            .HasForeignKey(p => p.RegionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ProfileImageHistory>()
            .HasOne(h => h.User)
            .WithMany()
            .HasForeignKey(h => h.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserActivity>()
            .HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed regions minimal example (will expand later)
        builder.Entity<MadaynRegion>().HasData(
            new MadaynRegion { RegionId = 1, RegionNameAr = "صحار", RegionNameEn = "Sohar", DescriptionAr = "المنطقة الحرة بصحار", DescriptionEn = "Sohar Free Zone", IsActive = true },
            new MadaynRegion { RegionId = 2, RegionNameAr = "الدقم", RegionNameEn = "Duqm", DescriptionAr = "المنطقة الاقتصادية الخاصة بالدقم", DescriptionEn = "Special Economic Zone at Duqm", IsActive = true },
            new MadaynRegion { RegionId = 3, RegionNameAr = "صلالة", RegionNameEn = "Salalah", DescriptionAr = "المنطقة الحرة بصلالة", DescriptionEn = "Salalah Free Zone", IsActive = true },
            new MadaynRegion { RegionId = 4, RegionNameAr = "المزيونة", RegionNameEn = "Al Mazunah", DescriptionAr = "المنطقة الحرة بالمزيونة", DescriptionEn = "Al Mazunah Free Zone", IsActive = true }
        );
    }
}
