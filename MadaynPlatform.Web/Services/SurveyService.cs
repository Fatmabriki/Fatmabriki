using Microsoft.EntityFrameworkCore;
using MadaynPlatform.Web.Data;
using MadaynPlatform.Web.Models;

namespace MadaynPlatform.Web.Services;

public class SurveyService : ISurveyService
{
    private readonly MadaynDbContext _dbContext;

    public SurveyService(MadaynDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Survey>> GetActiveSurveysAsync()
    {
        var now = DateTime.Now;
        return await _dbContext.Surveys
            .Where(s => !s.IsDeleted && s.IsActive && s.StartDate <= now && s.EndDate >= now)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<Survey?> GetSurveyWithQuestionsAsync(int id)
    {
        return await _dbContext.Surveys
            .Include(s => s.Questions.OrderBy(q => q.OrderIndex))
            .FirstOrDefaultAsync(s => s.SurveyId == id && !s.IsDeleted);
    }

    public async Task<UserResponse> StartOrGetResponseAsync(int surveyId, int? userId, string? guestSessionId)
    {
        var existing = await _dbContext.UserResponses
            .FirstOrDefaultAsync(r => r.SurveyId == surveyId && (userId != null ? r.UserId == userId : r.GuestSessionId == guestSessionId) && !r.IsCompleted);

        if (existing != null)
        {
            return existing;
        }

        var response = new UserResponse
        {
            SurveyId = surveyId,
            UserId = userId,
            GuestSessionId = userId == null ? guestSessionId : null,
            CreatedAt = DateTime.Now,
            IsCompleted = false
        };

        _dbContext.UserResponses.Add(response);
        await _dbContext.SaveChangesAsync();
        return response;
    }

    public async Task<bool> SubmitResponseAsync(int surveyId, int userResponseId, List<Answer> answers)
    {
        var response = await _dbContext.UserResponses
            .Include(r => r.Survey)
            .FirstOrDefaultAsync(r => r.UserResponseId == userResponseId && r.SurveyId == surveyId);
        if (response == null)
        {
            return false;
        }

        foreach (var answer in answers)
        {
            answer.UserResponseId = response.UserResponseId;
            _dbContext.Answers.Add(answer);
        }
        response.IsCompleted = true;
        response.CompletedAt = DateTime.Now;

        var survey = await _dbContext.Surveys.FirstAsync(s => s.SurveyId == surveyId);
        survey.ResponseCount += 1;

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CreateSurveyAsync(Survey survey, IEnumerable<Question> questions)
    {
        _dbContext.Surveys.Add(survey);
        await _dbContext.SaveChangesAsync();

        foreach (var question in questions)
        {
            question.SurveyId = survey.SurveyId;
            _dbContext.Questions.Add(question);
        }

        await _dbContext.SaveChangesAsync();
        return true;
    }
}

