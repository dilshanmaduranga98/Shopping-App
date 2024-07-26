using System.ComponentModel.DataAnnotations;

namespace ShoppingApp.FrontEnd.Models
{
    public class UserCartModel
    {
        public string userID { get; set; }
        public int productID { get; set; }
        public string name { get; set; }
        public double price { get; set; }
        public string description { get; set; }
        public string imageURL { get; set; }
        public double discount { get; set; }
        public int stock { get; set; }
        public int quantity { get; set; }
        public DateTime createDate { get; set; }
        public DateTime? updateDate { get; set; }

    }
}
