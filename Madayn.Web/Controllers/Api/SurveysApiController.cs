using Madayn.Web.Data;
using Madayn.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Madayn.Web.Controllers.Api;

[ApiController]
[Route("api/{culture:regex(^(ar|en)$)}/[controller]")]
public class SurveysApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public SurveysApiController(ApplicationDbContext context) { _context = context; }

    public record EvaluateDto(int SurveyId, int? RegionId, string? SessionId, int OverallRating, List<AnswerDto> Answers);
    public record AnswerDto(int QuestionId, int? Rating, string? Text, string? Option);

    [HttpGet("region/{regionId}")]
    public async Task<ActionResult<List<object>>> GetByRegion(int regionId, string culture)
    {
        bool isAr = culture == "ar";
        var list = await _context.Surveys.Where(s => s.IsActive && !s.IsHidden && !s.IsDeleted && s.RegionId == regionId)
            .Select(s => new
            {
                s.SurveyId,
                Title = isAr ? s.TitleAr : s.TitleEn,
                Description = isAr ? s.DescriptionAr : s.DescriptionEn,
                s.StartDate,
                s.EndDate
            }).ToListAsync();
        return Ok(list);
    }

    [HttpPost("evaluate")]
    public async Task<ActionResult> Evaluate(EvaluateDto dto)
    {
        var survey = await _context.Surveys.Include(s => s.Questions).FirstOrDefaultAsync(s => s.SurveyId == dto.SurveyId);
        if (survey == null || survey.IsDeleted || survey.IsHidden || !survey.IsActive) return NotFound();
        // Enforce time window
        var now = DateTime.UtcNow;
        if (survey.StartDate.HasValue && now < survey.StartDate.Value) return BadRequest("Not started");
        if (survey.EndDate.HasValue && now > survey.EndDate.Value) return BadRequest("Ended");

        var evaluation = new SurveyEvaluation
        {
            SurveyId = survey.SurveyId,
            RegionId = dto.RegionId,
            AnonymousSessionId = dto.SessionId,
            OverallRating = dto.OverallRating,
            EvaluatedAt = now,
            IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = Request.Headers["User-Agent"].ToString()
        };
        _context.SurveyEvaluations.Add(evaluation);
        await _context.SaveChangesAsync();

        if (dto.Answers != null)
        {
            foreach (var a in dto.Answers)
            {
                _context.SurveyAnswers.Add(new SurveyAnswer
                {
                    EvaluationId = evaluation.EvaluationId,
                    QuestionId = a.QuestionId,
                    AnswerRating = a.Rating,
                    AnswerText = a.Text,
                    SelectedOption = a.Option
                });
            }
            await _context.SaveChangesAsync();
        }

        // update aggregate
        var ratings = await _context.SurveyEvaluations.Where(e => e.SurveyId == survey.SurveyId).Select(e => e.OverallRating).ToListAsync();
        survey.TotalRatings = ratings.Count;
        survey.AverageRating = ratings.Count == 0 ? 0 : (decimal)Math.Round(ratings.Average(), 2);
        await _context.SaveChangesAsync();

        return Ok(new { success = true });
    }
}
