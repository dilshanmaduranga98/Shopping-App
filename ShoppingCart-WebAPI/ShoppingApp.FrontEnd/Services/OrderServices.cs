using Blazored.LocalStorage;
using Blazored.SessionStorage;
using ShoppingApp.FrontEnd.Interfaces;
using ShoppingApp.FrontEnd.Models;
using System.Diagnostics;
using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.JSInterop;




namespace ShoppingApp.FrontEnd.Services
{
    public class OrderServices:IOrderServices
    {
        private readonly HttpClient _httpClient;
        private readonly ISessionStorageService _sessionStorageService;
        private readonly ILocalStorageService _localStorageService;
        private readonly IJSRuntime _jsruntime;
        
        public OrderServices(HttpClient httpClient, ISessionStorageService sessionStorageService, ILocalStorageService localStorageService, IJSRuntime jSRuntime)
        {

            _httpClient = httpClient;
            _sessionStorageService = sessionStorageService;
            _localStorageService = localStorageService;
            _jsruntime = jSRuntime;

        }


        /// <summary>
        /// Adds a product to the user's cart.
        /// </summary>
        public async Task<string> AddToCart(UserProductModel userProducts)
        {
            try
            {
                // Get authentication token from session storage
                var token = await _sessionStorageService.GetItemAsync<string>("access_Token");

                // Set Bearer token in HTTP header
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Send POST request to add item to cart
                var result = await _httpClient.PostAsJsonAsync<UserProductModel>("https://localhost:7036/api/orders/managed-cart-items", userProducts);

                if(result.IsSuccessStatusCode) 
                {
                    return "200";
                }else
                {
                    return result.ToString();  // Return response details on failure
                }
            }catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// Retrieves all items in the user's cart.
        /// </summary>
        public async Task<CartInfoModel> ViewAllCartItems()
        {
            try
            {
                // Get authentication token from session storage
                var token = await _sessionStorageService.GetItemAsync<string>("access_Token");


                // Set Bearer token in HTTP header
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Send GET request to retrieve cart items
                var result = await _httpClient.GetFromJsonAsync<CartInfoModel>("https://localhost:7036/api/orders/managed-cart-items/cart-items");
               
                if (result != null)
                {
                    // Store cart items in local storage
                    await _localStorageService.SetItemAsync<CartInfoModel>("cartItems", result);
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
                
        }

        /// <summary>
        /// Updates the quantity of a specific item in the user's cart.
        /// </summary>
        public async Task<bool> UpdateCartItemQuntity(int productID, int newQuntity)
        {
            // Get authentication token from session storage
            var token = await _sessionStorageService.GetItemAsync<string>("access_Token");

            // Set Bearer token in HTTP header
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Construct URL for updating item quantity
            var url = $"api/orders/managed-cart-item/quantity?productId={productID}&newQuantity={newQuntity}";

            // Send PATCH request to update item quantity
            var result = await _httpClient.PatchAsync($"https://localhost:7036/{url}", null);


            // Return true if request is successful and response is not empty
            if (result.IsSuccessStatusCode && !string.IsNullOrEmpty(result.ToString()))
            {
                return true ;
            }else
            {
                return false;
            }

        }


        /// <summary>
        /// Deletes a specific item from the user's cart.
        /// </summary>
        public async Task<bool> DeleteCartItem(int productID)
        {
            // Get authentication token from session storage
            var token = await _sessionStorageService.GetItemAsync<string>("access_Token");


            // Set Bearer token in HTTP header
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Send DELETE request to delete item from cart
            var result = await _httpClient.DeleteAsync($"https://localhost:7036/api/orders/managed-cart-items/{productID}");


            // Return true if request is successful
            if (result.IsSuccessStatusCode)
            {
                return true;
            }else 
            {
                return false;
            }
        }


        /// <summary>
        /// Initiates the checkout process.
        /// </summary>
        public async Task<bool> CheckOut()
        {
            // Get authentication token from session storage
            var token = await _sessionStorageService.GetItemAsync<string>("access_Token");

            // Set Bearer token in HTTP header
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Send POST request to initiate checkout
            var result = await _httpClient.PostAsync("https://localhost:7036/api/payment/checkout", null);


            if(result.IsSuccessStatusCode)
            {
                // Deserialize response content to get checkout URL
                var content = await result.Content.ReadAsStringAsync();
                CheckoutUrlModel urlResult = JsonSerializer.Deserialize<CheckoutUrlModel>(content);
                
                Console.WriteLine($"checkout Url : {urlResult.url}");

                // Open the URL in a new browser tab using JSRuntime
                await _jsruntime.InvokeVoidAsync("openUrl", urlResult.url);
                return true;
            }else 
            { 
                return false; 
            }
        }


        /// <summary>
        /// Retrieves payment history data for the current user.
        /// </summary>
        public async Task<List<PaymentHistoryModel>> GetPaymentHistoryData()
        {

            // Get authentication token from session storage
            var token = await _sessionStorageService.GetItemAsync<string>("access_Token");

            // Set Bearer token in HTTP header
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Send GET request to retrieve payment history
            var result = await _httpClient.GetFromJsonAsync<List<PaymentHistoryModel>>("https://localhost:7036/api/payment/payment-history");
            
            //return true, if result not null
            if(result != null)
            {
                return result;
            }else
            {
                return null;
            }
                 
        }



        /// <summary>
        /// Retrieves order details for a specific order ID.
        /// </summary>
        public async Task<List<PayiedProductModel>> GetOrderDetails(int orderID)
        {
            // Get authentication token from session storage
            var token = await _sessionStorageService.GetItemAsync<string>("access_Token");


            // Set Bearer token in HTTP header
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


            // Send GET request to retrieve paid items for the specified order ID
            var result = await _httpClient.GetFromJsonAsync<List<PayiedProductModel>>($"https://localhost:7036/api/payment/paied-items/{orderID}");

            if(result!=null)
            {
                return result;
            }else
            {
                return null;
            }
        }
    }
}
