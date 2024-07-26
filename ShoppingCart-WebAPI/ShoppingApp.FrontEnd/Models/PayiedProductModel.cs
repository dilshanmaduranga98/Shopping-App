using System.ComponentModel.DataAnnotations;

namespace ShoppingApp.FrontEnd.Models
{
    public class PayiedProductModel
    {
        public string name { get; set; }

        public string imageURL { get; set; }

        public double price { get; set; }

        public int quantity { get; set; }

        public double totalPrice { get; set; }
    }
}
