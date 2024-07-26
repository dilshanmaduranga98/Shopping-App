using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ShoppingCart.Application.DTOs;
using ShoppingCart.Domain.Models;
using ShoppingCart.Infrastructure.Data;
using ShoppingCart.Application.Services.Interfaces;

namespace ShoppingCart.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ordersController : ControllerBase
    {
        private readonly IOrderServices _orderServices;


        // Constructor injection for dependencies.
        public ordersController(IOrderServices orderServices)
        {
            _orderServices = orderServices;
        }


        // AddToCart endpoint to add a product to the cart.HTTP POST method, requires authorization.
        /// <summary>
        /// Adds a product to the user's cart (for authorized users).
        /// </summary>
        /// <param name="productReq">The product details to be added.</param>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [HttpPost("managed-cart-items"), Authorize]
        public async Task<IActionResult> AddToCart(UserProductDTO productReq)
        {
            try
            {
                var result = await _orderServices.AddToCart(productReq);

                Log.Information("Item added to cart, Item data => {@result}", result);

                return Ok(result);     // Successful response with added item.



            }
            catch (Exception ex)
            {
                Log.Error("Item didn't add the cart!");

                return BadRequest(new { Mesage = "Error occured while try to add item to cart", Error = ex.Message
                });       // Bad request response with error message.
                throw new Exception("Error occured while try to add item to cart");
            }
        }

        // GetAllCartItems endpoint to retrieve all cart items.HTTP GET method, requires authorization.
        /// <summary>
        /// Retrieves all cart items for the authorized user.
        /// </summary>
        /// <returns>An IActionResult representing the list of cart items.</returns>
        //view all cart items 
        [HttpGet("managed-cart-items/cart-items"), Authorize]
        public async Task<IActionResult> GetAllCartItems()
        {
            try
            {
                var result = await _orderServices.ViewCartItem();

                Log.Information("View cart => {@result}", result);

                return Ok(result);


            }
            catch (Exception ex)
            {
                Log.Error("Error occured while get cart items!");

                return BadRequest(new { Message = "Error Occured while try to view cart items!", Error = ex.Message });
                throw new Exception("Error occured while get cart items!");
            }
        }


        // DeleteItem endpoint to remove a cart item. HTTP DELETE method, requires authorization.
        /// <summary>
        /// Deletes a cart item from the user's cart based on the specified product ID.
        /// </summary>
        /// <param name="productID">Enter the product ID to be removed from the cart.</param>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        //delete a cart item
        [HttpDelete("managed-cart-items/{productID}"), Authorize]
        public async Task<IActionResult> DeleteItem([FromRoute] int productID)
        {
            try
            {
                var result = await _orderServices.DeleteCartItem(productID);
                Log.Information("cart item deleted! item => {@result]", result);

                return Ok(new
                {
                    Message = "Item delete succesfully!",
                    Output = result
                });


            }
            catch (Exception ex)
            {
                Log.Error("Error occured while try to delete cart item!");
                return BadRequest(new
                {
                    Message = "Error occured while try to delete cart item!!",
                    Error = ex.Message
                });
                throw new Exception("Error occured while try to delete cart item!");
            }
        }



        // UpdateCartItemQuantity endpoint to update the quantity of a product in the cart. HTTP PATCH method, requires authorization.
        /// <summary>
        /// Updates the quantity of a product in the cart by its product ID.
        /// </summary>
        /// <param name="productId">The ID of the product to update.</param>
        /// <param name="newQuantity">The new quantity of the particular product.</param>
        /// <returns>An IActionResult representing the result of the update.</returns>
        [Authorize]
        [HttpPatch("managed-cart-item/quantity")]
        public async Task<IActionResult> UpdateCartItemQuantity(int productId, int newQuantity)
        {
            try
            {


                await _orderServices.UpdateCartItemQuantityAsync(productId, newQuantity);

                Log.Information("Item quantity updated!");

                return Ok("Cart item quantity updated successfully.");
            }
            catch (Exception ex)
            {
                Log.Error("Error occured while updating quantity!");
                return BadRequest(new { Message = "Error occured while updating quantity!", Error = ex.Message });
                throw new Exception("Error occured while updating quantity!");
            }
        }
    }
}
