using Microsoft.AspNetCore.Mvc;

namespace Mom_Project.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult DashboardPage()
        {
            return View();
        }
    }
}
