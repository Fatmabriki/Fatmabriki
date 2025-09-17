using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Madayn.Web.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("{culture:regex(^(ar|en)$)}/Admin/[controller]")]
public class ReportsController : Controller
{
    [HttpGet("survey/{id}")]
    public IActionResult Survey(int id)
    {
        ViewBag.SurveyId = id;
        return View();
    }
}
