using Moq;
using ShoppingCart.Application.Interfaces.IRepositories;
using ShoppingCart.Application.Services.Implementation;
using ShoppingCart.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartAPI.Test.Tests.ServicesTesting
{
    public class ProductServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _service = new ProductService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetProductByCategory_ShouldReturnProducts_WhenProductsExist()
        {
            // Arrange
            var categoryID = 1;
            var products = new List<Product> { new Product { categoryID = categoryID }, new Product { categoryID = categoryID } };
            _mockUnitOfWork.Setup(x => x.GetBaseRepository<Product>().GetByIDAsync(categoryID, a => a.categoryID == categoryID)).ReturnsAsync(products);

            // Act
            var result = await _service.GetProductByCategory(categoryID);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.All(result, product => Assert.Equal(categoryID, product.categoryID));
        }






        [Fact]
        public async Task ViewAllProducts_ShouldReturnAllProducts_WhenProductsExist()
        {
            // Arrange
            var products = new List<Product> { new Product { }, new Product { } };
            _mockUnitOfWork.Setup(x => x.GetBaseRepository<Product>().GetAll()).ReturnsAsync(products);

            // Act
            var result = await _service.ViewAllProducts();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task ViewProductByID_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var productID = 1;
            var product = new Product { productID = productID };
            _mockUnitOfWork.Setup(x => x.GetBaseRepository<Product>().GetById(productID)).ReturnsAsync(product);

            // Act
            var result = await _service.ViewProductByID(productID);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productID, result.Id);
        }


        [Fact]
        public async Task GetProductByCategory_ShouldReturnProducts_WhenCategoryExists()
        {
            // Arrange
            int categoryId = 1;
            var products = new List<Product> { new Product { categoryID = categoryId } };
            _mockUnitOfWork.Setup(u => u.GetBaseRepository<Product>().GetByIDAsync(categoryId, It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(products);

            // Act
            var result = await _service.GetProductByCategory(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(products, result);
        }

        [Fact]
        public async Task GetProductByCategory_ShouldThrowException_WhenCategoryNotExists()
        {
            // Arrange
            int categoryId = 999;
            _mockUnitOfWork.Setup(u => u.GetBaseRepository<Product>().GetByIDAsync(categoryId, It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync((List<Product>)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.GetProductByCategory(categoryId));
        }


        [Fact]
        public async Task ViewAllProducts_ShouldThrowException_WhenNoProductsExist()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.GetBaseRepository<Product>().GetAll())
                .ReturnsAsync((List<Product>)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.ViewAllProducts());
        }

        [Fact]
        public async Task ViewProductByID_ShouldThrowException_WhenProductNotExists()
        {
            // Arrange
            int productId = 999;
            _mockUnitOfWork.Setup(u => u.GetBaseRepository<Product>().GetById(productId))
                .ReturnsAsync((Product)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.ViewProductByID(productId));
        }

        [Fact]
        public async Task ViewAllProductCategory_ReturnsCategoryList()
        {
            // Arrange
            var expectedCategories = new List<ProductCategory>
        {
            new ProductCategory { categoryID = 1, name = "Category1" },
            new ProductCategory { categoryID = 2, name = "Category2" }
        };

            _mockUnitOfWork.Setup(repo => repo.GetBaseRepository<ProductCategory>().GetAll())
                .ReturnsAsync(expectedCategories);

            // Act
            var result = await _service.ViewAllProductCategory();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCategories.Count, result.Count);
            Assert.Equal(expectedCategories, result);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }
    }

}
