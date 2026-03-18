using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Mom_Project.Models;
using System.Data;

namespace Mom_Project.Controllers
{
    public class UserLoginController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Login(UserLoginModel model)
        {
            bool isValidUser = false;

            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(
                "Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;"))
                {
                    using (SqlCommand cmd = new SqlCommand("PR_MOM_User_Login", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@UserName", model.UserName);
                        cmd.Parameters.AddWithValue("@Password", model.Password);

                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                isValidUser = true;
                            }
                        }
                    }
                }
            }

            if (isValidUser)
            {
                HttpContext.Session.SetString("UserName", model.UserName);
                return RedirectToAction("DashboardPage", "Dashboard");
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password");
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    
    }
}
