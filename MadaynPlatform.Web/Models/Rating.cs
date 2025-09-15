using System.ComponentModel.DataAnnotations;

namespace MadaynPlatform.Web.Models;

public class Rating
{
    [Key]
    public int RatingId { get; set; }

    [Required, MaxLength(50)]
    public string EntityType { get; set; } = string.Empty; // Survey, News, Consultant, Contractor

    public int EntityId { get; set; }
    public int? UserId { get; set; }
    public string? GuestSessionId { get; set; }

    [Range(1, 5)]
    public int RatingValue { get; set; }

    [MaxLength(2000)]
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public User? User { get; set; }
}

