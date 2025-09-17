using System.ComponentModel.DataAnnotations;

namespace Madayn.Web.Models;

public class EmployeeEvaluation
{
    [Key] public int EvaluationId { get; set; }
    public int EmployeeId { get; set; } // who is evaluated (UserProfiles.UserId)
    public int EvaluatedBy { get; set; } // evaluator (UserProfiles.UserId)
    [Range(1,5)] public int PerformanceRating { get; set; }
    [Range(1,5)] public int TeamworkRating { get; set; }
    [Range(1,5)] public int CommunicationRating { get; set; }
    [MaxLength(1000)] public string? Comments { get; set; }
    [MaxLength(50)] public string? EvaluationPeriod { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsPublished { get; set; } = false; // published by admin
}
