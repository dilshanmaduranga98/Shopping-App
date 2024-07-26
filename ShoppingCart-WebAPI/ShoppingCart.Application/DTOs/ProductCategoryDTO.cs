
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Application.DTOs
{

    // Data Transfer Object (DTO) for product category.
    public class ProductCategoryDTO
    {
        public string userID { get; set; }

        [Required]
        public int productID {  get; set; }

        [Required]
        [MaxLength(50)]
        public string name {  get; set; }

        [Required]
        public double price { get; set; }

        [Required]
        [MaxLength(100)]
        public string description { get; set; }

        [Required]
        [MaxLength(250)]
        public string imageURL { get; set; }
        public double discount { get; set; }

        [Required]
        public int stock {  get; set; }

        [Required]
        public int quantity { get; set; }
        public DateTime createDate { get; set; }
        public DateTime? updateDate { get; set; }

        
    }
}
