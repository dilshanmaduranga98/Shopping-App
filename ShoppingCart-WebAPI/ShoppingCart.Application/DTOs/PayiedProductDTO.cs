
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Application.DTOs
{
    // Data Transfer Object (DTO) for paid product.
    public class PayiedProductDTO
    {
        [Required]
        [MaxLength(50)]
        public string name {  get; set; }

        public string imageURL {  get; set; }

        [Required]
        public double price { get; set; }

        [Required]
        public int quantity { get; set; }

        [Required]
        public double totalPrice { get; set; }
    }
}
