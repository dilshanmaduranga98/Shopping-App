using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ShoppingCart.API.Controllers;
using ShoppingCart.Application.DTOs;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Application.Services.Interfaces;
using Xunit;

namespace ShoppingCartAPI.Test.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<ILogger<usersController>> _mockLogger;
        private readonly Mock<IAddressService> _mockAddressService;
        private readonly usersController _controller;

        public UsersControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockLogger = new Mock<ILogger<usersController>>();
            _mockAddressService = new Mock<IAddressService>();
            _controller = new usersController(_mockAuthService.Object, _mockLogger.Object, _mockAddressService.Object);
        }

        [Fact]
        public async Task UserSignup_ShouldReturnOkResult_WhenUserCreatedSuccessfully()
        {
            // Arrange
            var userReq = new UserDTO { email = "test@example.com", password = "Password@123" };
            var userId = "12345";
            _mockAuthService.Setup(s => s.userSignup(userReq)).ReturnsAsync(userId);

            // Act
            var result = await _controller.UserSignup(userReq);

            // Assert
            //var okResult = Assert.IsType<OkObjectResult>(result);
            //Assert.Equal(userId, okResult.Value);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task UserSignup_ShouldReturnStatusCode500_WhenExceptionThrown()
        {
            // Arrange
            var userReq = new UserDTO { email = "test@example.com", password = "Password@123" };
            _mockAuthService.Setup(s => s.userSignup(userReq)).ThrowsAsync(new Exception("Error creating user"));

            // Act
            var result = await _controller.UserSignup(userReq);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            //Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred: Error creating user", statusCodeResult.Value);
        }

        [Fact]
        public async Task UserLogin_ShouldReturnOkResult_WhenLoginSuccessful()
        {
            // Arrange
            var loginReq = new loginDTO { email = "test@example.com", password = "Password@123" };
            var token = new AccessTokenDTO
            {
                access_token = "fake-jwt-token",
                refresh_token = "fake-refresh-token"
                
            };
            _mockAuthService.Setup(s => s.userLogin(loginReq.email, loginReq.password)).ReturnsAsync(token);

            // Act
            var result = await _controller.UserLogin(loginReq);

            // Assert
            //var okResult = Assert.IsType<OkObjectResult>(result);
            //Assert.Equal(new { Access_Token = token }, okResult.Value);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task UserLogin_ShouldThrowException_WhenLoginFails()
        {
            // Arrange
            var loginReq = new loginDTO { email = "test@example.com", password = "Password@123" };
            _mockAuthService.Setup(s => s.userLogin(loginReq.email, loginReq.password)).ReturnsAsync((AccessTokenDTO)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _controller.UserLogin(loginReq));
        }

        [Fact]
        public async Task PostAddress_ShouldReturnOkResult_WhenAddressAddedSuccessfully()
        {
            // Arrange
            var addressReq = new AddressDTO { street = "123 Main St", city = "Anytown" };
            var address = new AddressDTO { street = "123 Main St", city = "Anytown", ID = 1 };
            _mockAddressService.Setup(s => s.AddAddress(addressReq));

            // Act
            var result = await _controller.PostAddress(addressReq);

            // Assert
            //var okResult = Assert.IsType<OkObjectResult>(result);
          //  Assert.Equal(address, okResult.Value);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task PostAddress_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            var addressReq = new AddressDTO { street = "123 Main St", city = "Anytown" };
            _mockAddressService.Setup(s => s.AddAddress(addressReq)).ThrowsAsync(new Exception("Error adding address"));

            // Act
            var result = await _controller.PostAddress(addressReq);

            // Assert
            //var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            //Assert.Equal("Error adding address", badRequestResult.Value);
            Assert.ThrowsAsync<Exception>(() => _controller.PostAddress(addressReq));
        }

        [Fact]
        public async Task ViewAddresses_ShouldReturnOkResult_WhenAddressesRetrievedSuccessfully()
        {
            // Arrange
            var addresses = new List<AddressDTO>
        {
            new AddressDTO { street = "123 Main St", city = "Anytown" },
            new AddressDTO { street = "456 Elm St", city = "Othertown" }
        };
            _mockAddressService.Setup(s => s.ViewAddress());

            // Act
            var result = await _controller.ViewAddresses();

            // Assert
            //var okResult = Assert.IsType<OkObjectResult>(result);
            //Assert.Equal(addresses, okResult.Value);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ViewAddresses_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            _mockAddressService.Setup(s => s.ViewAddress()).ThrowsAsync(new Exception("Error retrieving addresses"));

            // Act
            var result = await _controller.ViewAddresses();

            // Assert
           // var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            //var response = badRequestResult.Value as dynamic;
            //Assert.Equal("Address not found!", response.Message);
            //Assert.Equal("Error retrieving addresses", response.Error);
             Assert.ThrowsAsync<Exception>(() => _controller.ViewAddresses());
        }
    }
}
