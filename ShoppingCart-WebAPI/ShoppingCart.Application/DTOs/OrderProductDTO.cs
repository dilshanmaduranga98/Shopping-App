using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Application.DTOs
{
    // Data Transfer Object (DTO) for Order product.
    public class OrderProductDTO
    {
        [Required]
        public int orderID { get; set; }

        [Required]
        public int productID { get; set; }

        [Required]
        public double unitPrice { get; set; }

        [Required]
        public int quantity { get; set; }
    }
}
