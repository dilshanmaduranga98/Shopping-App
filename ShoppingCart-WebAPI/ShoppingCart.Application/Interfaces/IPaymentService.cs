using ShoppingCart.Application.DTOs;
using ShoppingCart.Domain.Models;
using Stripe;
using Product = ShoppingCart.Domain.Models.Product;

namespace ShoppingCart.Application.Interfaces
{

    // Interface for payment-related services
    public interface IPaymentService
    {

        // Method to create a customer in Stripe
        Task<Customer> CreateCustomer(string name, string email);


        // Method for item checkout
        Task<string> ItemCheckout();


        // Method to retrieve payment history
        Task<List<PaymentHistoryDTO>> PaymentHistory();


        // Method to retrieve the product list of authorized users with order ID
        Task<List<PayiedProductDTO>> PaymentHistoryProducts(int orderId);
    }
}
