using MadaynPlatform.Web.Models;

namespace MadaynPlatform.Web.Services;

public interface IRatingService
{
    Task<double> RateAsync(string entityType, int entityId, int? userId, string? guestSessionId, int rating, string? comment);
    Task<double> GetAverageAsync(string entityType, int entityId);
}

