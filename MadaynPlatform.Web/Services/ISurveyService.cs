using MadaynPlatform.Web.Models;

namespace MadaynPlatform.Web.Services;

public interface ISurveyService
{
    Task<List<Survey>> GetActiveSurveysAsync();
    Task<Survey?> GetSurveyWithQuestionsAsync(int id);
    Task<UserResponse> StartOrGetResponseAsync(int surveyId, int? userId, string? guestSessionId);
    Task<bool> SubmitResponseAsync(int surveyId, int userResponseId, List<Answer> answers);
    Task<bool> CreateSurveyAsync(Survey survey, IEnumerable<Question> questions);
}

