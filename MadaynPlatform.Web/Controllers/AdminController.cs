using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MadaynPlatform.Web.Services;
using MadaynPlatform.Web.Models;

namespace MadaynPlatform.Web.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin")]
public class AdminController : Controller
{
    private readonly ISurveyService _surveyService;
    private readonly INewsService _newsService;

    public AdminController(ISurveyService surveyService, INewsService newsService)
    {
        _surveyService = surveyService;
        _newsService = newsService;
    }

    [HttpGet("Dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        return View();
    }

    [HttpGet("Surveys")]
    public async Task<IActionResult> Surveys()
    {
        var list = await _surveyService.GetActiveSurveysAsync();
        return View(list);
    }

    public class CreateSurveyViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(7);
    }

    [HttpPost("Surveys/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSurvey(CreateSurveyViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var survey = new Survey
        {
            Title = model.Title,
            Description = model.Description,
            EndDate = model.EndDate,
            CreatedByUserId = 1
        };
        await _surveyService.CreateSurveyAsync(survey, Enumerable.Empty<Question>());
        return RedirectToAction("Surveys");
    }

    [HttpGet("News")]
    public async Task<IActionResult> News()
    {
        var list = await _newsService.GetPublishedAsync();
        return View(list);
    }

    public class CreateNewsViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    [HttpPost("News/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateNews(CreateNewsViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var news = new News
        {
            Title = model.Title,
            Content = model.Content,
            CreatedByUserId = 1
        };
        await _newsService.CreateAsync(news);
        return RedirectToAction("News");
    }

    [HttpGet("Reports")]
    public async Task<IActionResult> Reports()
    {
        return View();
    }
}

