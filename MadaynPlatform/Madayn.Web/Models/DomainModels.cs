using System;
using System.Collections.Generic;

namespace Madayn.Web.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Investor"; // Admin, Investor, Guest
        public string? Company { get; set; }
        public string? Phone { get; set; }
        public string? Bio { get; set; }
        public string? ProfileImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }

        public ICollection<UserResponse> Responses { get; set; } = new List<UserResponse>();
    }

    public class Survey
    {
        public int SurveyId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }

    public class Question
    {
        public int QuestionId { get; set; }
        public int SurveyId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionType { get; set; } = "MultipleChoice"; // MultipleChoice, Text, Rating, YesNo
        public string? Options { get; set; } // JSON for multiple choice
        public bool IsRequired { get; set; } = true;
        public int Order { get; set; }

        public Survey? Survey { get; set; }
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }

    public class News
    {
        public int NewsId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsPublished { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? PublishedAt { get; set; }
    }

    public class Answer
    {
        public int AnswerId { get; set; }
        public int QuestionId { get; set; }
        public string? TextAnswer { get; set; }
        public int? ChoiceIndex { get; set; }
        public int? RatingValue { get; set; }
        public bool? YesNoValue { get; set; }
        public int? UserResponseId { get; set; }

        public Question? Question { get; set; }
        public UserResponse? UserResponse { get; set; }
    }

    public class UserResponse
    {
        public int UserResponseId { get; set; }
        public int UserId { get; set; }
        public int SurveyId { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.Now;
        public bool IsDraft { get; set; } = true;

        public User? User { get; set; }
        public Survey? Survey { get; set; }
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}

