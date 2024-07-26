
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Application.DTOs
{
    // Data Transfer Object (DTO) for payment history.
    public class PaymentHistoryDTO
    {
        public int order_id {  get; set; }
        public double total {  get; set; }

        public DateTime order_date { get; set; }


        [Required]
        [MaxLength(50)]
        public string order_status { get; set; }
    }
}
