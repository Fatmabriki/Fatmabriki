using Microsoft.AspNetCore.Mvc;

namespace Madayn.Web.Controllers
{
    public class InvestorController : Controller
    {
        public IActionResult Dashboard() => View();
        public IActionResult TakeSurvey() => View();
        public IActionResult MyResults() => View();
        public IActionResult NewsFeed() => View();
    }
}

