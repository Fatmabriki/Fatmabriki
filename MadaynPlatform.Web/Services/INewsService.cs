using MadaynPlatform.Web.Models;

namespace MadaynPlatform.Web.Services;

public interface INewsService
{
    Task<List<News>> GetPublishedAsync(int page = 1, int pageSize = 10);
    Task<News?> GetByIdAsync(int id);
    Task<News> CreateAsync(News news);
    Task<bool> PublishAsync(int id);
}

