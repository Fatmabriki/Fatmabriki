using Microsoft.AspNetCore.Mvc;
using MadaynPlatform.Web.Services;

namespace MadaynPlatform.Web.Controllers;

public class NewsController : Controller
{
    private readonly INewsService _newsService;
    private readonly IRatingService _ratingService;

    public NewsController(INewsService newsService, IRatingService ratingService)
    {
        _newsService = newsService;
        _ratingService = ratingService;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        var items = await _newsService.GetPublishedAsync(page);
        return View(items);
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await _newsService.GetByIdAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }
}

