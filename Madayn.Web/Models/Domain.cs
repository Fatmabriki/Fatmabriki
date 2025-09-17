using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Madayn.Web.Models;

public class Survey
{
    [Key] public int SurveyId { get; set; }
    [Required, MaxLength(300)] public string TitleAr { get; set; } = string.Empty;
    [Required, MaxLength(300)] public string TitleEn { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public int? RegionId { get; set; }
    public MadaynRegion? Region { get; set; }
    [MaxLength(50)] public string SurveyType { get; set; } = "Current";
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsArchived { get; set; }
    public bool IsHidden { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalRatings { get; set; }
    public List<SurveyQuestion> Questions { get; set; } = new();
}

public class SurveyQuestion
{
    [Key] public int QuestionId { get; set; }
    public int SurveyId { get; set; }
    public Survey? Survey { get; set; }
    [Required, MaxLength(1000)] public string QuestionTextAr { get; set; } = string.Empty;
    [Required, MaxLength(1000)] public string QuestionTextEn { get; set; } = string.Empty;
    [MaxLength(50)] public string QuestionType { get; set; } = "Rating";
    public string? OptionsAr { get; set; }
    public string? OptionsEn { get; set; }
    public bool IsRequired { get; set; } = true;
    public int? OrderIndex { get; set; }
}

public class News
{
    [Key] public int NewsId { get; set; }
    [Required, MaxLength(300)] public string TitleAr { get; set; } = string.Empty;
    [Required, MaxLength(300)] public string TitleEn { get; set; } = string.Empty;
    public string ContentAr { get; set; } = string.Empty;
    public string ContentEn { get; set; } = string.Empty;
    [MaxLength(500)] public string? ImageUrl { get; set; }
    public int? RegionId { get; set; }
    public MadaynRegion? Region { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsHidden { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int ViewCount { get; set; }
    public decimal AverageRating { get; set; }
}

public class Consultant
{
    [Key] public int ConsultantId { get; set; }
    [Required, MaxLength(200)] public string NameAr { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string NameEn { get; set; } = string.Empty;
    [MaxLength(200)] public string? SpecializationAr { get; set; }
    [MaxLength(200)] public string? SpecializationEn { get; set; }
    [MaxLength(200)] public string? CompanyAr { get; set; }
    [MaxLength(200)] public string? CompanyEn { get; set; }
    public int? RegionId { get; set; }
    public MadaynRegion? Region { get; set; }
    [MaxLength(200)] public string? Contact { get; set; }
    [MaxLength(200)] public string? Email { get; set; }
    [MaxLength(500)] public string? ImageUrl { get; set; }
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public bool IsActive { get; set; } = true;
    public decimal AverageRating { get; set; }
}

public class Contractor
{
    [Key] public int ContractorId { get; set; }
    [Required, MaxLength(250)] public string CompanyNameAr { get; set; } = string.Empty;
    [Required, MaxLength(250)] public string CompanyNameEn { get; set; } = string.Empty;
    [MaxLength(200)] public string? SpecializationAr { get; set; }
    [MaxLength(200)] public string? SpecializationEn { get; set; }
    public int? RegionId { get; set; }
    public MadaynRegion? Region { get; set; }
    [MaxLength(200)] public string? Contact { get; set; }
    [MaxLength(200)] public string? Email { get; set; }
    [MaxLength(300)] public string? Website { get; set; }
    [MaxLength(500)] public string? ImageUrl { get; set; }
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public bool IsActive { get; set; } = true;
    public decimal AverageRating { get; set; }
}

public class Comment
{
    [Key] public int CommentId { get; set; }
    [Required, MaxLength(50)] public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public int? UserId { get; set; }
    [MaxLength(100)] public string? AnonymousName { get; set; }
    [Required, MaxLength(2000)] public string CommentText { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public bool IsHidden { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
}

public class Rating
{
    [Key] public int RatingId { get; set; }
    [Required, MaxLength(50)] public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public int? RatedBy { get; set; }
    [MaxLength(100)] public string? AnonymousSessionId { get; set; }
    [Range(1,5)] public int Score { get; set; }
    [MaxLength(1000)] public string? Review { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class AuditLog
{
    [Key] public int LogId { get; set; }
    public int? UserId { get; set; }
    [MaxLength(50)] public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    [MaxLength(20)] public string Action { get; set; } = string.Empty;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    [MaxLength(50)] public string? IPAddress { get; set; }
    [MaxLength(500)] public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class AnonymousSession
{
    [Key, MaxLength(100)] public string SessionId { get; set; } = string.Empty;
    [MaxLength(50)] public string? IPAddress { get; set; }
    [MaxLength(500)] public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;
}
