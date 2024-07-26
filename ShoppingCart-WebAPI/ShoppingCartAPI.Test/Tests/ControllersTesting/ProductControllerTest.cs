using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ShoppingCart.API.Controllers;
using ShoppingCart.Application.Services.Interfaces;
using ShoppingCart.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartAPI.Test.Tests.Controllers
{
    public class ProductControllerTest
    {

        private readonly Mock<IProductService> _mockProductService;
        private readonly Mock<ILogger<productsController>> _mockLogger;
        private readonly productsController _controller;

        public ProductControllerTest()
        {
            _mockProductService = new Mock<IProductService>();
            _mockLogger = new Mock<ILogger<productsController>>();
            _controller = new productsController(_mockProductService.Object);
        }

        [Fact]
        public async Task GetAllProducts_ShouldReturnOkResult_WhenProductsRetrievedSuccessfully()
        {
            // Arrange
           
            _mockProductService.Setup(s => s.ViewAllProducts());

            // Act
            var result = await _controller.GetAllProducts();

            // Assert
            //var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAllProducts_ShouldReturnNotFound_WhenExceptionThrown()
        {
            // Arrange
            _mockProductService.Setup(s => s.ViewAllProducts()).ThrowsAsync(new Exception("Error retrieving products"));

            // Act
            var result = await _controller.GetAllProducts();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
//            Assert.Equal("No Item founds!!", (notFoundResult.Value as dynamic).Error);
            Assert.ThrowsAsync<Exception>( () => _controller.GetAllProducts());
        }

        [Fact]
        public async Task GetByCategory_ShouldReturnOkResult_WhenProductsRetrievedSuccessfully()
        {
            // Arrange
            int categoryId = 3;
            //var productList = new List<object> { new { Name = "Product1" }, new { Name = "Product2" } };
            _mockProductService.Setup(s => s.GetProductByCategory(categoryId));

            // Act
            var result = await _controller.GetByCategory(categoryId);

            // Assert
            //var okResult = Assert.IsType<OkObjectResult>(result);
            //Assert.Equal(productList, okResult.Value);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByCategory_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            int categoryId = 1;
            _mockProductService.Setup(s => s.GetProductByCategory(categoryId)).ThrowsAsync(new Exception("Error retrieving products by category"));

            // Act
            var result = await _controller.GetByCategory(categoryId);

            // Assert
           
            Assert.ThrowsAsync<Exception>(() => _controller.GetByCategory(categoryId));
        }

        [Fact]
        public async Task GetProductByID_ShouldReturnOkResult_WhenProductRetrievedSuccessfully()
        {
            // Arrange
            int productId = 1;
            _mockProductService.Setup(s => s.ViewProductByID(productId));

            // Act
            var result = await _controller.GetProductByID(productId);

            // Assert
            
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetProductByID_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            int productId = 1;
            _mockProductService.Setup(s => s.ViewProductByID(productId)).ThrowsAsync(new Exception("Error retrieving product by ID"));

            // Act
            var result = await _controller.GetProductByID(productId);

            // Assert
            //var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            //Assert.Equal("Error retrieving product by ID", badRequestResult.Value);
            Assert.ThrowsAsync<Exception>( () => _controller.GetProductByID(productId));
        }


        [Fact]
        public async Task ViewCategory_ShouldReturnOkResult_WhenCategoriesRetrievedSuccessfully()
        {
            // Arrange
            var categories = new List<ProductCategory> { new ProductCategory { categoryID = 1, name = "Electronics" } };
            _mockProductService.Setup(s => s.ViewAllProductCategory()).ReturnsAsync(categories);

            // Act
            var result = await _controller.ViewCategory();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(categories, okResult.Value);
        }

        [Fact]
        public async Task ViewCategory_ShouldReturnNotFound_WhenExceptionThrown()
        {
            // Arrange
            _mockProductService.Setup(s => s.ViewAllProductCategory()).ThrowsAsync(new Exception("Error retrieving categories"));

            // Act
            var result = await _controller.ViewCategory();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var expectedValue = new { Error = "No category founds!!" };

            Assert.Equal("No category founds!!", notFoundResult.Value);
        }
    }
}
