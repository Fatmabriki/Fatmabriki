using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MadaynPlatform.Web.Models;

// نموذج المستخدم الداخلي للتطبيق (ليس بديل IdentityUser)
// يُستخدم لملف التعريف ومعلومات إضافية مرتبطة بـ IdentityUser عبر UserId (FK)
public class User
{
    [Key]
    public int UserId { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Role { get; set; } = "Investor"; // Admin, Investor, Guest

    [MaxLength(200)]
    public string? Company { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(2000)]
    public string? Bio { get; set; }

    [MaxLength(500)]
    public string? ProfileImageUrl { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    // Navigation Properties
    public ICollection<Survey> CreatedSurveys { get; set; } = new List<Survey>();
    public ICollection<UserResponse> UserResponses { get; set; } = new List<UserResponse>();
    public ICollection<News> CreatedNews { get; set; } = new List<News>();
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}

