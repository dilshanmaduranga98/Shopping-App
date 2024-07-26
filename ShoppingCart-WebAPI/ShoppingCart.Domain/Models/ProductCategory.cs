
using System.Text.Json.Serialization;

namespace ShoppingCart.Domain.Models
{
    //model for product catehgory tabel
    public class ProductCategory
    {
        public int categoryID { get; set; }
        public string name { get; set; }
        public string imageURL { get; set; }


        //get collection of products



        public ICollection<Product> Products { get; set; }      // Collection of products associated with this category.
    }
}
