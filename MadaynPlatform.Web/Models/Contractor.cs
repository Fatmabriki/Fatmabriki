using System.ComponentModel.DataAnnotations;

namespace MadaynPlatform.Web.Models;

public class Contractor
{
    [Key]
    public int ContractorId { get; set; }

    [Required, MaxLength(250)]
    public string CompanyName { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Specialization { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(200)]
    public string? Contact { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public double AverageRating { get; set; } = 0;

    public User CreatedBy { get; set; } = null!;
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}

