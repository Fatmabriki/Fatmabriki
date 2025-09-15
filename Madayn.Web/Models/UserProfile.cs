using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Madayn.Web.Models;

public class UserProfile
{
    [Key]
    public int UserId { get; set; }

    [Required]
    [MaxLength(450)]
    public string AspNetUserId { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public string? FullName { get; private set; }

    [MaxLength(200)]
    public string? DisplayName { get; set; }

    [MaxLength(1000)]
    public string? Bio { get; set; }

    [MaxLength(100)]
    public string? Department { get; set; }

    [MaxLength(100)]
    public string? Position { get; set; }

    [MaxLength(200)]
    public string? Company { get; set; }

    public int? RegionId { get; set; }
    public MadaynRegion? Region { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? Website { get; set; }

    [MaxLength(500)]
    public string? LinkedIn { get; set; }

    [MaxLength(500)]
    public string? Twitter { get; set; }

    [MaxLength(500)]
    public string? ProfileImageUrl { get; set; }

    [MaxLength(500)]
    public string? ProfileImageThumbnailUrl { get; set; }

    // Privacy
    public bool ShowEmail { get; set; } = false;
    public bool ShowPhone { get; set; } = false;
    public bool ShowSocialMedia { get; set; } = true;
    public bool AllowMessages { get; set; } = true;

    // Notifications
    public bool EmailNotifications { get; set; } = true;
    public bool SurveyNotifications { get; set; } = true;
    public bool NewsNotifications { get; set; } = true;
    public bool CommentNotifications { get; set; } = true;

    // Status
    public bool IsActive { get; set; } = true;
    public bool IsApproved { get; set; } = false;
    public bool IsVerified { get; set; } = false;
    public DateTime? LastLoginAt { get; set; }
    public int LoginCount { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastProfileUpdate { get; set; }
}

public class ProfileImageHistory
{
    [Key]
    public int ImageId { get; set; }
    public int UserId { get; set; }
    public UserProfile? User { get; set; }
    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;
    public long ImageSize { get; set; }
    [MaxLength(100)]
    public string MimeType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class UserActivity
{
    [Key]
    public int ActivityId { get; set; }
    public int UserId { get; set; }
    public UserProfile? User { get; set; }
    [MaxLength(50)]
    public string ActivityType { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? Description { get; set; }
    [MaxLength(50)]
    public string? IPAddress { get; set; }
    [MaxLength(500)]
    public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class MadaynRegion
{
    [Key]
    public int RegionId { get; set; }
    [Required]
    [MaxLength(100)]
    public string RegionNameAr { get; set; } = string.Empty;
    [Required]
    [MaxLength(100)]
    public string RegionNameEn { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? DescriptionAr { get; set; }
    [MaxLength(500)]
    public string? DescriptionEn { get; set; }
    public bool IsActive { get; set; } = true;
}
