using Madayn.Web.Data;
using Madayn.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Madayn.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class RatingsApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public RatingsApiController(ApplicationDbContext context) { _context = context; }

    public record RateDto(string EntityType, int EntityId, int Score, string? Review, string? SessionId);

    [HttpPost("")]
    public async Task<IActionResult> Rate(RateDto dto)
    {
        if (dto.Score < 1 || dto.Score > 5) return BadRequest();
        var rating = new Rating
        {
            EntityType = dto.EntityType,
            EntityId = dto.EntityId,
            Score = dto.Score,
            Review = dto.Review,
            AnonymousSessionId = dto.SessionId
        };
        _context.Ratings.Add(rating);
        await _context.SaveChangesAsync();

        // Update aggregates for supported entities
        async Task update<T>(DbSet<T> set, Func<T, bool> predicate, Action<T, decimal, int> apply) where T : class
        {
            var r = await _context.Ratings.Where(x => x.EntityType == dto.EntityType && x.EntityId == dto.EntityId).ToListAsync();
            var avg = r.Count == 0 ? 0 : (decimal)Math.Round(r.Average(x => x.Score), 2);
            var count = r.Count;
            var entity = set.Local.FirstOrDefault(predicate) ?? set.FirstOrDefault(predicate);
            if (entity != null) { apply(entity, avg, count); await _context.SaveChangesAsync(); }
        }

        if (dto.EntityType == nameof(News))
        {
            await update(_context.News, n => n.NewsId == dto.EntityId, (n, avg, _) => n.AverageRating = avg);
        }
        else if (dto.EntityType == nameof(Consultant))
        {
            await update(_context.Consultants, c => c.ConsultantId == dto.EntityId, (c, avg, _) => c.AverageRating = avg);
        }
        else if (dto.EntityType == nameof(Contractor))
        {
            await update(_context.Contractors, c => c.ContractorId == dto.EntityId, (c, avg, _) => c.AverageRating = avg);
        }

        return Ok(new { success = true });
    }
}
