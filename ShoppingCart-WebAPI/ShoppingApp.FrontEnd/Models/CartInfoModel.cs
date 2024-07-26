namespace ShoppingApp.FrontEnd.Models
{
    public class CartInfoModel
    {
        public string userID { get; set; }
        public List<UserCartModel> cartItems { get; set; }
        public DateTime? cart_create_date { get; set; }
        public DateTime? cart_update_date { get; set; }
        public int totalItems { get; set; }
        public double subTotal { get; set; }
    }
}
