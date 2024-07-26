using System.Text.Json.Serialization;

namespace ShoppingCart.Domain.Models
{
    //model for address table
    public class Address
    {
        public int addressID {  get; set; }
        public string street {  get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string postalCode { get; set; }
        public bool IsPrimary { get; set; }



        // Property for user ID as a foreign key.
        public User User { get; set; }
        public string userID { get; set; }

    }
}
