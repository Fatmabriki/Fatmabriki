using System.ComponentModel.DataAnnotations;

namespace MadaynPlatform.Web.Models;

public class UserResponse
{
    [Key]
    public int UserResponseId { get; set; }
    public int SurveyId { get; set; }
    public int? UserId { get; set; }
    public string? GuestSessionId { get; set; }
    public bool IsCompleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? CompletedAt { get; set; }

    public Survey Survey { get; set; } = null!;
    public User? User { get; set; }
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}

