using System.ComponentModel.DataAnnotations;

namespace ShoppingApp.FrontEnd.Models
{
    public class LoginModel
    {
        [Required]
        [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$", ErrorMessage = "Email should be in correct format.")]
        public string email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)[A-Za-z\d@#$%^&+=]{8,12}$", ErrorMessage = "Password must be 8-12 characters long and include at least one uppercase letter, one lowercase letter, one digit, and may include special characters @#$%^&+=")]
        public string password { get; set; }
    }
}
