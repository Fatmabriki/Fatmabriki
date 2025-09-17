using Madayn.Web.Data;
using Madayn.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Madayn.Web.Services;

public interface IProfileService
{
    Task<UserProfile?> GetProfileByAspNetUserIdAsync(string aspNetUserId);
    Task<UserProfile?> GetProfileAsync(int userId);
    Task<int> GetActivityCountAsync(int userId);
    Task UpdateProfileAsync(int userId, EditProfileViewModel model);
    Task UpdatePrivacySettingsAsync(int userId, PrivacySettingsViewModel model);
    Task UpdateNotificationSettingsAsync(int userId, NotificationSettingsViewModel model);
    Task LogActivityAsync(int userId, string type, string description);
    Task<UserProfile> EnsureProfileForAspNetUserAsync(string aspNetUserId, string? email);
}

public class ProfileService : IProfileService
{
    private readonly ApplicationDbContext _context;

    public ProfileService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfile?> GetProfileByAspNetUserIdAsync(string aspNetUserId)
    {
        return await _context.UserProfiles.Include(p => p.Region)
            .FirstOrDefaultAsync(p => p.AspNetUserId == aspNetUserId);
    }

    public async Task<UserProfile?> GetProfileAsync(int userId)
    {
        return await _context.UserProfiles.Include(p => p.Region).FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<int> GetActivityCountAsync(int userId)
    {
        return await _context.UserActivities.CountAsync(a => a.UserId == userId);
    }

    public async Task UpdateProfileAsync(int userId, EditProfileViewModel model)
    {
        var profile = await _context.UserProfiles.FindAsync(userId);
        if (profile == null) throw new InvalidOperationException("Profile not found");

        profile.FirstName = model.FirstName;
        profile.LastName = model.LastName;
        profile.DisplayName = model.DisplayName;
        profile.Bio = model.Bio;
        profile.Position = model.Position;
        profile.Company = model.Company;
        profile.RegionId = model.RegionId;
        profile.Phone = model.Phone;
        profile.Website = model.Website;
        profile.LinkedIn = model.LinkedIn;
        profile.Twitter = model.Twitter;
        profile.ShowEmail = model.ShowEmail;
        profile.ShowPhone = model.ShowPhone;
        profile.ShowSocialMedia = model.ShowSocialMedia;
        profile.AllowMessages = model.AllowMessages;
        if (!string.IsNullOrWhiteSpace(model.ProfileImageUrl))
        {
            profile.ProfileImageUrl = model.ProfileImageUrl;
            profile.ProfileImageThumbnailUrl = model.ProfileImageThumbnailUrl;
        }
        profile.UpdatedAt = DateTime.UtcNow;
        profile.LastProfileUpdate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePrivacySettingsAsync(int userId, PrivacySettingsViewModel model)
    {
        var profile = await _context.UserProfiles.FindAsync(userId) ?? throw new InvalidOperationException("Profile not found");
        profile.ShowEmail = model.ShowEmail;
        profile.ShowPhone = model.ShowPhone;
        profile.ShowSocialMedia = model.ShowSocialMedia;
        profile.AllowMessages = model.AllowMessages;
        await _context.SaveChangesAsync();
    }

    public async Task UpdateNotificationSettingsAsync(int userId, NotificationSettingsViewModel model)
    {
        var profile = await _context.UserProfiles.FindAsync(userId) ?? throw new InvalidOperationException("Profile not found");
        profile.EmailNotifications = model.EmailNotifications;
        profile.SurveyNotifications = model.SurveyNotifications;
        profile.NewsNotifications = model.NewsNotifications;
        profile.CommentNotifications = model.CommentNotifications;
        await _context.SaveChangesAsync();
    }

    public async Task LogActivityAsync(int userId, string type, string description)
    {
        _context.UserActivities.Add(new UserActivity
        {
            UserId = userId,
            ActivityType = type,
            Description = description,
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
    }

    public async Task<UserProfile> EnsureProfileForAspNetUserAsync(string aspNetUserId, string? email)
    {
        var existing = await GetProfileByAspNetUserIdAsync(aspNetUserId);
        if (existing != null) return existing;

        string fallbackName = (email ?? "user@local").Split('@').FirstOrDefault() ?? "User";
        var profile = new UserProfile
        {
            AspNetUserId = aspNetUserId,
            FirstName = fallbackName,
            LastName = "",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync();
        return profile;
    }
}

public class ProfileViewModel
{
    public UserProfile Profile { get; set; } = null!;
    public bool CanEdit { get; set; }
    public bool ShowContactInfo { get; set; }
    public ActivityCountViewModel ActivityCount { get; set; } = new ActivityCountViewModel();
}

public class EditProfileViewModel
{
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;
    [System.ComponentModel.DataAnnotations.StringLength(200)]
    public string? DisplayName { get; set; }
    [System.ComponentModel.DataAnnotations.StringLength(1000)]
    public string? Bio { get; set; }
    [System.ComponentModel.DataAnnotations.StringLength(100)]
    public string? Position { get; set; }
    [System.ComponentModel.DataAnnotations.StringLength(200)]
    public string? Company { get; set; }
    public int? RegionId { get; set; }
    [System.ComponentModel.DataAnnotations.Phone]
    public string? Phone { get; set; }
    [System.ComponentModel.DataAnnotations.Url]
    public string? Website { get; set; }
    [System.ComponentModel.DataAnnotations.Url]
    public string? LinkedIn { get; set; }
    [System.ComponentModel.DataAnnotations.Url]
    public string? Twitter { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? ProfileImageThumbnailUrl { get; set; }
    public bool ShowEmail { get; set; }
    public bool ShowPhone { get; set; }
    public bool ShowSocialMedia { get; set; } = true;
    public bool AllowMessages { get; set; } = true;
}

public class PrivacySettingsViewModel
{
    public bool ShowEmail { get; set; }
    public bool ShowPhone { get; set; }
    public bool ShowSocialMedia { get; set; } = true;
    public bool AllowMessages { get; set; } = true;
}

public class NotificationSettingsViewModel
{
    public bool EmailNotifications { get; set; } = true;
    public bool SurveyNotifications { get; set; } = true;
    public bool NewsNotifications { get; set; } = true;
    public bool CommentNotifications { get; set; } = true;
}

public class ActivityCountViewModel
{
    public int SurveysCompleted { get; set; }
    public int CommentsPosted { get; set; }
    public int RatingsGiven { get; set; }
}
