using System.ComponentModel.DataAnnotations;

namespace Mom_Project.Models
{
    public class UserLoginModel
    {
        [Required(ErrorMessage = "User name is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
