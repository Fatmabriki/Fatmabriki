using System.ComponentModel.DataAnnotations;

namespace MadaynPlatform.Web.Models;

public class News
{
    [Key]
    public int NewsId { get; set; }

    [Required, MaxLength(250)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public bool IsPublished { get; set; } = false;
    public bool IsDeleted { get; set; } = false;
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? PublishedAt { get; set; }
    public double AverageRating { get; set; } = 0;
    public int ViewCount { get; set; } = 0;

    public User CreatedBy { get; set; } = null!;
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}

