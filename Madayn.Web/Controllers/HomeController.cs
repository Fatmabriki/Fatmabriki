using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Madayn.Web.Models;
using Microsoft.Extensions.Localization;

namespace Madayn.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public HomeController(ILogger<HomeController> logger, IStringLocalizer<SharedResource> localizer)
    {
        _logger = logger;
        _localizer = localizer;
    }

    [Route("{culture:regex(^(ar|en)$)}/[controller]/[action]")]
    [Route("{culture:regex(^(ar|en)$)}/[action]")]
    [Route("{culture:regex(^(ar|en)$)}")]
    [Route("")]
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
