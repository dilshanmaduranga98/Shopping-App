using ShoppingCart.Application.DTOs;
using ShoppingCart.Domain.Models;

namespace ShoppingCart.Application.Services.Interfaces
{

    // Interface for order-related services
    public interface IOrderServices
    {

        // Method to create a new order
        Task<Order> GetOrder(double orderTotal, string orderStatus, string paymentID, string sessionId, string cutomerID);


        // Method to add an item to the cart
        Task<string> AddToCart(UserProductDTO product);


        // Method to view all cart items
        Task<object> ViewCartItem();


        // Method to delete a cart item by ID
        Task<UserCart> DeleteCartItem(int id);


        // Method to update the quantity of a cart item
        Task<UserCart> UpdateCartItemQuantityAsync(int productId, int newQuantity);

    }
}
