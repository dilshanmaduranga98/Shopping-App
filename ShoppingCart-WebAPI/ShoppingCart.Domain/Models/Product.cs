
using System.Text.Json.Serialization;

namespace ShoppingCart.Domain.Models
{
    //model for Product tabel
    public class Product
    {
        public int productID { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string imageURL { get; set; }
        public double price { get; set; }
        public double discount { get; set; }
        public int stock {  get; set; }


        //add categoryID as foreign key
        public ProductCategory ProductCategory { get; set; }      // Navigation property for the related product category.
        public int categoryID { get; set; }


        public ICollection<UserCart> UserCart { get; set; }     // Collection of user carts associated with this product.


        public ICollection<CurrentOrders> CurrentOrders { get; set; }     // Collection of current orders associated with this product.


    }
}
