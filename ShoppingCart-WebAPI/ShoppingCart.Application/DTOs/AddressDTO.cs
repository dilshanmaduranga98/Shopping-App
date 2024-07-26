
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Application.DTOs
{
    //data tarnsfer object of Addresses
    public class AddressDTO
    {
        public int ID { get; set; }
        [Required]
        [MaxLength(50)]
        public string street { get; set; }

        [Required]
        [MaxLength(50)]
        public string city { get; set; }

        [Required]
        [MaxLength(50)]
        public string country { get; set; }

        [Required]
        [MaxLength(10)]
        public string postalCode { get; set; }
    }
}
