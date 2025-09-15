using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MadaynPlatform.Web.Services;
using MadaynPlatform.Web.Models;

namespace MadaynPlatform.Web.Controllers;

[Route("Survey")]
public class SurveyController : Controller
{
    private readonly ISurveyService _surveyService;
    private readonly IRatingService _ratingService;

    public SurveyController(ISurveyService surveyService, IRatingService ratingService)
    {
        _surveyService = surveyService;
        _ratingService = ratingService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var surveys = await _surveyService.GetActiveSurveysAsync();
        return View(surveys);
    }

    [HttpGet("Take/{id}")]
    public async Task<IActionResult> TakeSurvey(int id)
    {
        var survey = await _surveyService.GetSurveyWithQuestionsAsync(id);
        if (survey == null) return NotFound();
        var response = await _surveyService.StartOrGetResponseAsync(id, null, HttpContext.Session.Id);
        ViewData["UserResponseId"] = response.UserResponseId;
        return View(survey);
    }

    public class SurveySubmissionViewModel
    {
        public int UserResponseId { get; set; }
        public List<Answer> Answers { get; set; } = new();
    }

    [HttpPost("Submit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitSurvey(int id, SurveySubmissionViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var ok = await _surveyService.SubmitResponseAsync(id, model.UserResponseId, model.Answers);
        if (!ok) return NotFound();
        return RedirectToAction("ThankYou");
    }

    [HttpGet("ThankYou")]
    public IActionResult ThankYou()
    {
        return View();
    }

    [HttpPost("Rate/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RateSurvey(int id, int rating, string comment)
    {
        var avg = await _ratingService.RateAsync("Survey", id, null, HttpContext.Session.Id, rating, comment);
        return Json(new { success = true, average = avg });
    }
}

