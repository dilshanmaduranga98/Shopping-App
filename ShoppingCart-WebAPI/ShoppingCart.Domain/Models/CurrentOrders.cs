
using System.Text.Json.Serialization;

namespace ShoppingCart.Domain.Models
{
    // Model for joint table of Order and Product tables.
    public class CurrentOrders //OrderProduct 
    {

        public Order Order { get; set; }        // Navigation property for the related order.
        public int orderID { get; set; }



        public Product Product { get; set; }    // Navigation property for the related product.
        public int productID { get; set; }


        public double unitPrice { get; set; }
        public int orderQuantity { get; set; }


    }
}
