using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Application.DTOs
{
    // Data Transfer Object (DTO) for order.
    public class OrderDTO
    {
        [Required]
        public DateTime orderDate { get; set; }

        [Required]
        public double orderTotal { get; set; }

        [Required]
        [MaxLength(10)]
        public Boolean orderSatus { get; set; }

        [Required]
        [MaxLength(60)]
        public string? paymentID { get; set; } = "1";
    }
}
