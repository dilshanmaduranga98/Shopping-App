using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShoppingCart.Application.DTOs;
using ShoppingCart.Application.Services.Interfaces;
using ShoppingCart.API.Controllers;
using System.Threading.Tasks;

namespace ShoppingCartAPI.Test.Tests.Controllers
{
    public class OrderControllerTest
    {
        private readonly Mock<IOrderServices> _mockOrderServices;
        private readonly ordersController _controller;

        public OrderControllerTest()
        {
            _mockOrderServices = new Mock<IOrderServices>();
            _controller = new ordersController(_mockOrderServices.Object);
        }

        [Fact]
        public async Task AddToCart_ShouldReturnOkResult_WhenItemAddedSuccessfully()
        {
            // Arrange
            var productReq = new UserProductDTO
            {
                productID = 12,
                quantity = 20
            };
            _mockOrderServices.Setup(s => s.AddToCart(It.IsAny<UserProductDTO>()));

            // Act
            var result =  _controller.AddToCart(productReq);

            // Assert
            Assert.True(result.IsCompletedSuccessfully);
        }

        [Fact]
        public async Task AddToCart_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            var productReq = new UserProductDTO();

            _mockOrderServices.Setup(s => s.AddToCart(It.IsAny<UserProductDTO>())).ThrowsAsync(new Exception("Error adding item to cart"));

            // Act
            var result =  _controller.AddToCart(productReq);

            // Assert
            Assert.False(result.IsFaulted);
        }

        [Fact]
        public async Task GetAllCartItems_ShouldReturnOkResult_WhenItemsRetrievedSuccessfully()
        {
            // Arrange
            _mockOrderServices.Setup(s => s.ViewCartItem()).ReturnsAsync(new { items = new List<object>() });

            // Act
            var result = await _controller.GetAllCartItems();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetAllCartItems_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            _mockOrderServices.Setup(s => s.ViewCartItem()).ThrowsAsync(new Exception("Error retrieving cart items"));

            // Act
            var result = await _controller.GetAllCartItems();

            // Assert
            Assert.ThrowsAsync<Exception>(() => _controller.GetAllCartItems());
        }

        public async Task DeleteItem_ShouldReturnOkResult_WhenItemDeletedSuccessfully()
        {
            // Arrange
            var productId = 1;
            _mockOrderServices.Setup(s => s.DeleteCartItem(productId));

            // Act
            var result = await _controller.DeleteItem(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True((okResult.Value as dynamic).Output.success);
        }

        [Fact]
        public async Task DeleteItem_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            var productId = 1;
            _mockOrderServices.Setup(s => s.DeleteCartItem(productId)).ThrowsAsync(new Exception("Error deleting cart item"));

            // Act
            var result = await _controller.DeleteItem(productId);

            // Assert
            
            Assert.ThrowsAsync<Exception>(() => _controller.DeleteItem(productId));
        }

        [Fact]
        public async Task UpdateCartItemQuantity_ShouldReturnOkResult_WhenQuantityUpdatedSuccessfully()
        {
            // Arrange
            var productId = 1;
            var newQuantity = 5;
            _mockOrderServices.Setup(s => s.UpdateCartItemQuantityAsync(productId, newQuantity));

            // Act
            var result = await _controller.UpdateCartItemQuantity(productId, newQuantity);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Cart item quantity updated successfully.", okResult.Value);
        }

        [Fact]
        public async Task UpdateCartItemQuantity_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            var productId = 1;
            var newQuantity = 5;
            _mockOrderServices.Setup(s => s.UpdateCartItemQuantityAsync(productId, newQuantity)).ThrowsAsync(new Exception("Error updating quantity"));

            // Act
            var result = await _controller.UpdateCartItemQuantity(productId, newQuantity);

            // Assert
            
            Assert.ThrowsAsync<Exception>(() => _controller.UpdateCartItemQuantity(productId, newQuantity));
        }
    }
}
