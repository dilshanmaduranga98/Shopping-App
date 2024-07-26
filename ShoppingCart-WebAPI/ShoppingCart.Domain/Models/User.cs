
using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShoppingCart.Domain.Models
{

    //model for user tabel
    public class User
    {
        public string userID { get; set; }
        
        public string firstname { get; set; }


        public string lastname { get; set; }

        
        public string email { get; set; }


        
        public int phonenumber { get; set; }

        public string strip_CustomerID { get; set; } = null;




        //get collection of Oders
        public ICollection<Order> Orders { get; set; }   // Collection of orders associated with this user.

        //get colle ction of Addresses
        public ICollection<Address> Addresss { get; set; }    // Collection of addresses associated with this user.


        //get collection of userProducts from user and product joint tabel
        public ICollection<UserCart> UserCart { get; set; }    // Collection of user products associated with this user.
    }
}
