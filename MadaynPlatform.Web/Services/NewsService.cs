using Microsoft.EntityFrameworkCore;
using MadaynPlatform.Web.Data;
using MadaynPlatform.Web.Models;

namespace MadaynPlatform.Web.Services;

public class NewsService : INewsService
{
    private readonly MadaynDbContext _dbContext;

    public NewsService(MadaynDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<News>> GetPublishedAsync(int page = 1, int pageSize = 10)
    {
        return await _dbContext.News
            .Where(n => n.IsPublished && !n.IsDeleted)
            .OrderByDescending(n => n.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public Task<News?> GetByIdAsync(int id)
    {
        return _dbContext.News.FirstOrDefaultAsync(n => n.NewsId == id && !n.IsDeleted);
    }

    public async Task<News> CreateAsync(News news)
    {
        _dbContext.News.Add(news);
        await _dbContext.SaveChangesAsync();
        return news;
    }

    public async Task<bool> PublishAsync(int id)
    {
        var news = await _dbContext.News.FirstOrDefaultAsync(n => n.NewsId == id);
        if (news == null)
        {
            return false;
        }
        news.IsPublished = true;
        news.PublishedAt = DateTime.Now;
        await _dbContext.SaveChangesAsync();
        return true;
    }
}

