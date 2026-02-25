using Microsoft.AspNetCore.Mvc;

namespace Mom_Project.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult RegisterPage()
        {
            return View();
        }
    }
}
