using System.ComponentModel.DataAnnotations;

namespace MadaynPlatform.Web.Models;

public class Question
{
    [Key]
    public int QuestionId { get; set; }
    public int SurveyId { get; set; }

    [Required, MaxLength(2000)]
    public string QuestionText { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string QuestionType { get; set; } = "MultipleChoice"; // MultipleChoice, Text, Rating, YesNo

    // JSON options for MultipleChoice
    public string? Options { get; set; }

    public bool IsRequired { get; set; } = true;
    public int OrderIndex { get; set; }

    public Survey Survey { get; set; } = null!;
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}

