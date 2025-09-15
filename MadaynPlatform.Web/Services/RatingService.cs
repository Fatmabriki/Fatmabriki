using Microsoft.EntityFrameworkCore;
using MadaynPlatform.Web.Data;
using MadaynPlatform.Web.Models;

namespace MadaynPlatform.Web.Services;

public class RatingService : IRatingService
{
    private readonly MadaynDbContext _dbContext;

    public RatingService(MadaynDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<double> RateAsync(string entityType, int entityId, int? userId, string? guestSessionId, int rating, string? comment)
    {
        var entry = new Rating
        {
            EntityType = entityType,
            EntityId = entityId,
            UserId = userId,
            GuestSessionId = userId == null ? guestSessionId : null,
            RatingValue = rating,
            Comment = comment,
            CreatedAt = DateTime.Now
        };
        _dbContext.Ratings.Add(entry);
        await _dbContext.SaveChangesAsync();

        var avg = await GetAverageAsync(entityType, entityId);

        // update average on related entity
        switch (entityType)
        {
            case "Survey":
                var survey = await _dbContext.Surveys.FirstOrDefaultAsync(s => s.SurveyId == entityId);
                if (survey != null) { survey.AverageRating = avg; }
                break;
            case "News":
                var news = await _dbContext.News.FirstOrDefaultAsync(n => n.NewsId == entityId);
                if (news != null) { news.AverageRating = avg; }
                break;
            case "Consultant":
                var cons = await _dbContext.Consultants.FirstOrDefaultAsync(c => c.ConsultantId == entityId);
                if (cons != null) { cons.AverageRating = avg; }
                break;
            case "Contractor":
                var cont = await _dbContext.Contractors.FirstOrDefaultAsync(c => c.ContractorId == entityId);
                if (cont != null) { cont.AverageRating = avg; }
                break;
        }
        await _dbContext.SaveChangesAsync();
        return avg;
    }

    public async Task<double> GetAverageAsync(string entityType, int entityId)
    {
        var ratings = await _dbContext.Ratings
            .Where(r => r.EntityType == entityType && r.EntityId == entityId)
            .ToListAsync();
        if (ratings.Count == 0) return 0;
        return ratings.Average(r => r.RatingValue);
    }
}

