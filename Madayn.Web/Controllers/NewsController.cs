using Madayn.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Madayn.Web.Controllers;

[Route("{culture:regex(^(ar|en)$)}/[controller]")]
public class NewsController : BaseController
{
    private readonly ApplicationDbContext _context;
    public NewsController(Microsoft.Extensions.Localization.IStringLocalizer<SharedResource> localizer, ApplicationDbContext context) : base(localizer)
    { _context = context; }

    [HttpGet("")]
    public async Task<IActionResult> Index(int? regionId)
    {
        var isAr = GetCurrentLanguage() == "ar";
        var q = _context.News.Where(n => !n.IsDeleted && !n.IsHidden && n.IsPublished);
        if (regionId.HasValue) q = q.Where(n => n.RegionId == regionId);
        var data = await q.OrderByDescending(n => n.PublishedAt)
            .Select(n => new
            {
                n.NewsId,
                Title = isAr ? n.TitleAr : n.TitleEn,
                Content = isAr ? n.ContentAr : n.ContentEn,
                n.ImageUrl,
                n.PublishedAt,
                n.RegionId
            }).ToListAsync();
        ViewBag.Items = data;
        ViewBag.Regions = await _context.MadaynRegions.Where(r => r.IsActive).ToListAsync();
        return View();
    }

    [HttpGet("details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var isAr = GetCurrentLanguage() == "ar";
        var n = await _context.News.FirstOrDefaultAsync(x => x.NewsId == id && !x.IsDeleted && !x.IsHidden && x.IsPublished);
        if (n == null) return NotFound();
        ViewBag.Item = new
        {
            n.NewsId,
            Title = isAr ? n.TitleAr : n.TitleEn,
            Content = isAr ? n.ContentAr : n.ContentEn,
            n.ImageUrl,
            n.PublishedAt,
            n.RegionId
        };
        return View();
    }
}
