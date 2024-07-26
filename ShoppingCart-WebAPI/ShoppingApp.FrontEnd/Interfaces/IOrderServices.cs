using ShoppingApp.FrontEnd.Models;

namespace ShoppingApp.FrontEnd.Interfaces
{
    public interface IOrderServices
    {
        Task<string> AddToCart(UserProductModel userProducts);

        Task<CartInfoModel> ViewAllCartItems();
        Task<bool> UpdateCartItemQuntity(int productID, int newQuntity);
        Task<bool> DeleteCartItem(int productID);
        Task<bool> CheckOut();
        Task<List<PaymentHistoryModel>> GetPaymentHistoryData();

        Task<List<PayiedProductModel>> GetOrderDetails(int orderID);
    }
}
