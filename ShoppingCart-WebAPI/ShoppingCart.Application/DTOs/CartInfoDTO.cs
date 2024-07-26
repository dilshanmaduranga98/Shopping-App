using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Application.DTOs
{
    public class CartInfoDTO
    {
       public string userID { get; set; }
        public List<ProductCategoryDTO> cartItems { get; set; }
        public DateTime? cart_create_date { get; set; }
        public DateTime? cart_update_date { get; set; }
        public int totalItems { get; set; }
        public double subTotal { get; set; }
    }
}
