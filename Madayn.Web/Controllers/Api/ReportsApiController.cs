using Madayn.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
}
