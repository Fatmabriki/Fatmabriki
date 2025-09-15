using Microsoft.AspNetCore.Mvc;

namespace Madayn.Web.Controllers
{
    public class AdminController : Controller
    {
        // لوحة التحكم - إحصائيات وبطاقات سريعة
        public IActionResult Dashboard()
        {
            return View();
        }

        // إدارة المستخدمين
        public IActionResult Users()
        {
            return View();
        }

        // إدارة الاستبيانات
        public IActionResult Surveys()
        {
            return View();
        }

        // إدارة الأخبار
        public IActionResult News()
        {
            return View();
        }

        // التقارير والرسوم البيانية
        public IActionResult Reports()
        {
            return View();
        }
    }
}

