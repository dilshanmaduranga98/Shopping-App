
using System.Text.Json.Serialization;

namespace ShoppingCart.Domain.Models
{
    //model for order table
    public class Order
    {
        public int orderID {  get; set; }
        public DateTime orderDate { get; set; }
        public double orderTotal { get; set; }
        public string orderSatus { get; set; }
        public string SessionID { get; set; }
        public string paymentID { get; set; } = null;


        //add userID as foreign key
        
        public User User { get; set; }  // Navigation property for the related user.
        public string userID { get; set; }


       
        public ICollection<CurrentOrders> CurrentOrders { get; set; }   // Collection of current orders associated with this order.

    }
}
