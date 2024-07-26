using System.ComponentModel.DataAnnotations;

namespace ShoppingApp.FrontEnd.Models
{
    public class UserModel
    {
            public string? userID { get; set; } = "";

            [Required]
            [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$", ErrorMessage = "Email should be in correct format.")]
            public string? email { get; set; }

            [Required]
            [RegularExpression(@"^\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number.")]
            [Display(Name = "Phone Number" )]
            public int phoneNumber { get; set; }

            [Required]
            public string? firstName { get; set; }

            [Required]
            public string? LastName { get; set; }

            [Required]
            [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)[A-Za-z\d@#$%^&+=]{8,12}$", ErrorMessage = "Password must be 8-12 characters long and include at least one uppercase letter, one lowercase letter, one digit, and may include special characters @#$%^&+=")]
            public string? password { get; set; }

            [Required]
            public string? confirmPassword { get; set; }

            public AddressModel addressDTO { get; set; } = new AddressModel();

       
    }

}
