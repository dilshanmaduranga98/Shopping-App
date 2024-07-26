using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Application.DTOs
{
    //data tarnsfer object of user login
    public class loginDTO
    {
        [Required]
        [MaxLength(50)]
        public string email {  get; set; }

        [Required]
        [MaxLength(20)]
        public string password { get; set; }
    }
}
