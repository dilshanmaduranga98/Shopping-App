
using System.Text.Json.Serialization;

namespace ShoppingCart.Domain.Models
{
    // Model for joint table for User and Product table.
    public class UserCart
    {

        public User User { get; set; }            // User associated with this user cart.
        public string userID { get; set; }


        public Product Product { get; set; }      // Product associated with this user cart.
        public int productID { get; set; }

        public int quantity { get; set; }

        public DateTime createDate { get; set; } 

        public DateTime? updateDate { get; set; }

        public DateTime cartCreateDate { get; set; }

        public DateTime? cartUpdateDate { get; set; }
    }
}
