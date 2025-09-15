using Microsoft.AspNetCore.Mvc;

namespace Madayn.Web.Controllers
{
    public class NewsController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Create() => View();
        public IActionResult Edit(int id) => View();
    }
}

