using Madayn.Web.Data;
using Madayn.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Madayn.Web.Controllers.Employee;

[Authorize(Roles = "Employee,Admin")]
[Route("{culture:regex(^(ar|en)$)}/Employee/[controller]")]
public class PeerEvaluationController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    public PeerEvaluationController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    { _context = context; _userManager = userManager; }

    [HttpGet("evaluate/{employeeId}")]
    public IActionResult EvaluateColleague(int employeeId)
    {
        ViewBag.EmployeeId = employeeId;
        return View();
    }

    [HttpPost("evaluate/{employeeId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitPeerEvaluation(int employeeId, int performanceRating, int teamworkRating, int communicationRating, string? comments)
    {
        var aspId = _userManager.GetUserId(User)!;
        var evaluatorProfile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.AspNetUserId == aspId);
        if (evaluatorProfile == null) return Unauthorized();
        _context.EmployeeEvaluations.Add(new EmployeeEvaluation
        {
            EmployeeId = employeeId,
            EvaluatedBy = evaluatorProfile.UserId,
            PerformanceRating = performanceRating,
            TeamworkRating = teamworkRating,
            CommunicationRating = communicationRating,
            Comments = comments,
            EvaluationPeriod = DateTime.UtcNow.ToString("yyyy-MM")
        });
        await _context.SaveChangesAsync();
        return RedirectToAction("MyEvaluations");
    }

    [HttpGet("my-evaluations")]
    public async Task<IActionResult> MyEvaluations()
    {
        var aspId = _userManager.GetUserId(User)!;
        var me = await _context.UserProfiles.FirstOrDefaultAsync(p => p.AspNetUserId == aspId);
        var list = await _context.EmployeeEvaluations.Where(e => e.EvaluatedBy == me!.UserId).OrderByDescending(e => e.CreatedAt).ToListAsync();
        return View(list);
    }
}
