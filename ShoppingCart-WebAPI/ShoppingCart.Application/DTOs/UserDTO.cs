
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingCart.Application.DTOs
{
    //data tarnsfer object of new User
    public class UserDTO
    {
        public string? userID { get; set; }

        [Required(ErrorMessage = "Email required!")]
        [MaxLength(50)]
        [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$", ErrorMessage = "Email should be in correct format.")]
        public string? email { get; set; }


        [Required(ErrorMessage = "Phonenumber required!")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number.")]
        public int phoneNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string? firstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string? LastName { get; set; }

        [NotMapped]
        [MaxLength(20)]
        [Required(ErrorMessage = "Password Required!")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)[A-Za-z\d@#$%^&+=]{8,12}$", ErrorMessage = "Password must be 8-12 characters long and include at least one uppercase letter, one lowercase letter, one digit, and may include special characters @#$%^&+=")]
        public string? password { get; set; }


        [NotMapped]
        [Required]
        [MaxLength(20)]
        public string? confirmPassword { get; set; }

        public AddressDTO addressDTO { get; set; }

    }
}
