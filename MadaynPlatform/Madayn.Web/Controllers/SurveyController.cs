using Microsoft.AspNetCore.Mvc;

namespace Madayn.Web.Controllers
{
    public class SurveyController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Take(int id) => View();
        public IActionResult Manage() => View();
    }
}

