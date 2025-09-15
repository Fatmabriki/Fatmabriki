using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MadaynPlatform.Web.Models;

namespace MadaynPlatform.Web.Data;

// DbContext رئيسي للتطبيق مع دعم Identity
public class MadaynDbContext : IdentityDbContext<IdentityUser>
{
    public MadaynDbContext(DbContextOptions<MadaynDbContext> options) : base(options)
    {
    }

    public DbSet<User> UsersExt { get; set; } = null!; // User profile table
    public DbSet<Survey> Surveys { get; set; } = null!;
    public DbSet<Question> Questions { get; set; } = null!;
    public DbSet<Answer> Answers { get; set; } = null!;
    public DbSet<UserResponse> UserResponses { get; set; } = null!;
    public DbSet<News> News { get; set; } = null!;
    public DbSet<Consultant> Consultants { get; set; } = null!;
    public DbSet<Contractor> Contractors { get; set; } = null!;
    public DbSet<Rating> Ratings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // relationships
        modelBuilder.Entity<Survey>()
            .HasOne(s => s.CreatedBy)
            .WithMany(u => u.CreatedSurveys)
            .HasForeignKey(s => s.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Question>()
            .HasOne(q => q.Survey)
            .WithMany(s => s.Questions)
            .HasForeignKey(q => q.SurveyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Answer>()
            .HasOne(a => a.Question)
            .WithMany(q => q.Answers)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Answer>()
            .HasOne(a => a.UserResponse)
            .WithMany(r => r.Answers)
            .HasForeignKey(a => a.UserResponseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserResponse>()
            .HasOne(r => r.Survey)
            .WithMany(s => s.UserResponses)
            .HasForeignKey(r => r.SurveyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserResponse>()
            .HasOne(r => r.User)
            .WithMany(u => u.UserResponses)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<News>()
            .HasOne(n => n.CreatedBy)
            .WithMany(u => u.CreatedNews)
            .HasForeignKey(n => n.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Consultant>()
            .HasOne(c => c.CreatedBy)
            .WithMany()
            .HasForeignKey(c => c.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Contractor>()
            .HasOne(c => c.CreatedBy)
            .WithMany()
            .HasForeignKey(c => c.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Rating>()
            .HasOne(r => r.User)
            .WithMany(u => u.Ratings)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // seed minimal admin role and user placeholder via Identity stored in migration step.
    }
}

