using System.ComponentModel.DataAnnotations;

namespace MadaynPlatform.Web.Models;

public class Survey
{
    [Key]
    public int SurveyId { get; set; }

    [Required, MaxLength(250)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string? Description { get; set; }

    public DateTime StartDate { get; set; } = DateTime.Now;
    public DateTime EndDate { get; set; } = DateTime.Now.AddDays(7);
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public double AverageRating { get; set; } = 0;
    public int ResponseCount { get; set; } = 0;

    public User CreatedBy { get; set; } = null!;
    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<UserResponse> UserResponses { get; set; } = new List<UserResponse>();
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}

