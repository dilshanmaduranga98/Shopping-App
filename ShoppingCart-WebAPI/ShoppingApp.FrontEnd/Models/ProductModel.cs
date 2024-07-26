namespace ShoppingApp.FrontEnd.Models
{
    public class ProductModel
    {
        public int productID { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string imageURL { get; set; }
        public double price { get; set; }
        public double discount { get; set; }
        public int stock { get; set; }
        public int categoryID { get; set; }
    }
}
