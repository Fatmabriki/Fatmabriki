using System.ComponentModel.DataAnnotations;

namespace MadaynPlatform.Web.Models;

public class Answer
{
    [Key]
    public int AnswerId { get; set; }
    public int QuestionId { get; set; }
    public int UserResponseId { get; set; }

    [MaxLength(4000)]
    public string AnswerText { get; set; } = string.Empty;
    public int? AnswerValue { get; set; }

    public Question Question { get; set; } = null!;
    public UserResponse UserResponse { get; set; } = null!;
}

