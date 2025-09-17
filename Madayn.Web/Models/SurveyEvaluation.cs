using System.ComponentModel.DataAnnotations;

namespace Madayn.Web.Models;

public class SurveyEvaluation
{
    [Key] public int EvaluationId { get; set; }
    public int SurveyId { get; set; }
    public Survey? Survey { get; set; }
    public int? UserId { get; set; } // registered user
    public string? AnonymousSessionId { get; set; } // guest
    public int? RegionId { get; set; }
    public MadaynRegion? Region { get; set; }
    [Range(1,5)] public int OverallRating { get; set; }
    public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
    public string? IPAddress { get; set; }
    public string? UserAgent { get; set; }
    public List<SurveyAnswer> Answers { get; set; } = new();
}

public class SurveyAnswer
{
    [Key] public int AnswerId { get; set; }
    public int EvaluationId { get; set; }
    public SurveyEvaluation? Evaluation { get; set; }
    public int QuestionId { get; set; }
    public SurveyQuestion? Question { get; set; }
    public string? AnswerText { get; set; }
    [Range(1,5)] public int? AnswerRating { get; set; }
    public string? SelectedOption { get; set; }
}
