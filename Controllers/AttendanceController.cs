using Microsoft.AspNetCore.Mvc;

namespace Mom_Project.Controllers
{
    public class AttendanceController : Controller
    {
        public IActionResult AttendanceList()
        {
            return View();
        }
    }
}