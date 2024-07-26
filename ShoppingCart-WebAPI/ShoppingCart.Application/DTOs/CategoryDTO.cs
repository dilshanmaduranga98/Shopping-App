
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Application.DTOs
{
    //data tarnsfer object of Producyt Category
    public class CategoryDTO
    {
        [Required]
        [MaxLength(50)]
        public string name { get; set; }

        [Required]
        [MaxLength(250)]
        public string imageURL { get; set; }
    }
}
