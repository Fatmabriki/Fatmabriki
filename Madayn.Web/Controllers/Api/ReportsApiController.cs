using Madayn.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Madayn.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ReportsApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public ReportsApiController(ApplicationDbContext context) { _context = context; }

    [HttpGet("survey/{id}/regional-breakdown")]
    public async Task<IActionResult> RegionalBreakdown(int id)
    {
        var data = await _context.SurveyEvaluations
            .Where(e => e.SurveyId == id)
            .GroupBy(e => e.RegionId)
            .Select(g => new { RegionId = g.Key, Avg = g.Average(x => x.OverallRating), Count = g.Count() })
            .ToListAsync();
        var regions = await _context.MadaynRegions.ToDictionaryAsync(r => r.RegionId, r => new { r.RegionNameAr, r.RegionNameEn });
        return Ok(new
        {
            regions = data.Select(d => regions.ContainsKey(d.RegionId ?? 0) ? regions[d.RegionId ?? 0].RegionNameAr : "-").ToList(),
            ratings = data.Select(d => Math.Round(d.Avg, 2)).ToList(),
            counts = data.Select(d => d.Count).ToList()
        });
    }

    [HttpGet("survey/{id}/questions-breakdown")]
    public async Task<IActionResult> QuestionsBreakdown(int id, string? culture = null)
    {
        var isAr = (culture ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName) == "ar";
        var questions = await _context.SurveyQuestions.Where(q => q.SurveyId == id).OrderBy(q => q.OrderIndex).ToListAsync();

        // Preload role mappings for registered users
        var userIdToRole = await (from p in _context.UserProfiles
                                  join ur in _context.UserRoles on p.AspNetUserId equals ur.UserId
                                  join r in _context.Roles on ur.RoleId equals r.Id
                                  select new { p.UserId, r.Name }).ToListAsync();
        var roleByUser = userIdToRole.GroupBy(x => x.UserId).ToDictionary(g => g.Key, g => g.Select(x => x.Name).ToHashSet());

        var evals = await _context.SurveyEvaluations.Where(e => e.SurveyId == id).Select(e => new { e.EvaluationId, e.UserId, e.AnonymousSessionId }).ToListAsync();
        var evalIdToKind = evals.ToDictionary(e => e.EvaluationId, e => e.UserId.HasValue ? (roleByUser.TryGetValue(e.UserId.Value, out var roles) && roles.Contains("Employee") ? "Employee" : "Investor") : "Guest");

        var answers = await _context.SurveyAnswers.Where(a => a.Evaluation!.SurveyId == id).ToListAsync();

        var result = new List<object>();
        foreach (var q in questions)
        {
            var qAnswers = answers.Where(a => a.QuestionId == q.QuestionId);
            int count1 = 0, count2 = 0, count3 = 0, count4 = 0, count5 = 0;
            int g1 = 0, g2 = 0, g3 = 0, g4 = 0, g5 = 0;
            int inv1 = 0, inv2 = 0, inv3 = 0, inv4 = 0, inv5 = 0;
            int emp1 = 0, emp2 = 0, emp3 = 0, emp4 = 0, emp5 = 0;
            int textCount = 0;
            foreach (var a in qAnswers)
            {
                var kind = evalIdToKind.TryGetValue(a.EvaluationId, out var k) ? k : "Guest";
                if (a.AnswerRating.HasValue)
                {
                    switch (a.AnswerRating.Value)
                    {
                        case 1: count1++; if (kind=="Guest") g1++; else if (kind=="Investor") inv1++; else emp1++; break;
                        case 2: count2++; if (kind=="Guest") g2++; else if (kind=="Investor") inv2++; else emp2++; break;
                        case 3: count3++; if (kind=="Guest") g3++; else if (kind=="Investor") inv3++; else emp3++; break;
                        case 4: count4++; if (kind=="Guest") g4++; else if (kind=="Investor") inv4++; else emp4++; break;
                        case 5: count5++; if (kind=="Guest") g5++; else if (kind=="Investor") inv5++; else emp5++; break;
                    }
                }
                if (!string.IsNullOrWhiteSpace(a.AnswerText)) textCount++;
            }
            result.Add(new
            {
                questionId = q.QuestionId,
                text = isAr ? q.QuestionTextAr : q.QuestionTextEn,
                ratings = new[] { count1, count2, count3, count4, count5 },
                guest = new[] { g1, g2, g3, g4, g5 },
                investor = new[] { inv1, inv2, inv3, inv4, inv5 },
                employee = new[] { emp1, emp2, emp3, emp4, emp5 },
                textCount
            });
        }
        return Ok(result);
    }
}
