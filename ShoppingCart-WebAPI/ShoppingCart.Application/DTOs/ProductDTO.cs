
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Application.DTOs
{
    //data tarnsfer object of new product
    public class ProductDTO
    {
        [Required]
        [MaxLength(50)]
        public string name { get; set; }

        [Required]
        [MaxLength(100)]
        public string description { get; set; }

        [Required]
        [MaxLength(250)]
        public string imageURL { get; set; }

        [Required]
        public double price { get; set; }
        public double discount { get; set; }

        [Required]
        public int stock { get; set; }

        [Required]
        public int categoryID { get; set; }
    }
}
