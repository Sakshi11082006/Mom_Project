using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Mom_Project.Models;

namespace Mom_Project.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult tables_data()
        {
            return View();
        }
        public IActionResult tables_general()
        {
            return View();
        }
    }
}
