using Microsoft.AspNetCore.Mvc;
using MadaynPlatform.Web.Services;
using MadaynPlatform.Web.Models;

namespace MadaynPlatform.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApiController : ControllerBase
{
    private readonly ISurveyService _surveyService;
    private readonly IRatingService _ratingService;

    public ApiController(ISurveyService surveyService, IRatingService ratingService)
    {
        _surveyService = surveyService;
        _ratingService = ratingService;
    }

    public record SurveyDto(int SurveyId, string Title, double AverageRating, int ResponseCount);
    public record RatingDto(int RatingValue, string? Comment);
    public record SurveyStatsDto(int TotalSurveys, int TotalResponses, double AverageRating);
    public record ConsultantDto(int ConsultantId, string Name, string Specialization, double AverageRating);

    [HttpGet("surveys")]
    public async Task<ActionResult<List<SurveyDto>>> GetSurveys()
    {
        var list = await _surveyService.GetActiveSurveysAsync();
        return list.Select(s => new SurveyDto(s.SurveyId, s.Title, s.AverageRating, s.ResponseCount)).ToList();
    }

    [HttpPost("surveys/{id}/rate")]
    public async Task<ActionResult> RateSurvey(int id, RatingDto rating)
    {
        var avg = await _ratingService.RateAsync("Survey", id, null, HttpContext.Session.Id, rating.RatingValue, rating.Comment);
        return Ok(new { average = avg });
    }

    [HttpGet("reports/survey-stats")]
    public async Task<ActionResult<SurveyStatsDto>> GetSurveyStats()
    {
        var list = await _surveyService.GetActiveSurveysAsync();
        var totalResponses = list.Sum(s => s.ResponseCount);
        var avg = list.Count == 0 ? 0 : list.Average(s => s.AverageRating);
        return new SurveyStatsDto(list.Count, totalResponses, avg);
    }

    [HttpGet("consultants")]
    public async Task<ActionResult<List<ConsultantDto>>> GetConsultants()
    {
        // Placeholder: empty list for now
        return new List<ConsultantDto>();
    }
}

