using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Application.Interfaces.IRepositories;
using ShoppingCart.Application.Services.Implementation;
using ShoppingCart.Domain.Models;
using ShoppingCart.Application.DTOs;
using System.Security.Claims;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ShoppingCart.Infrastructure.Repositories;
using ShoppingCart.Application.Services.Interfaces;
using System.Collections;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Serilog;
using FluentAssertions;

namespace ShoppingCartAPI.Test.Tests.ServicesTesting
{
    public class OrderServiceTest
    {
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<ITokenServices> _mockTokenServices;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly OrderServices _orderServices;
        private readonly Mock<IUpdateTimeStampService> _mockUpdateTimeStampService;

        public OrderServiceTest()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockTokenServices = new Mock<ITokenServices>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUpdateTimeStampService = new Mock<IUpdateTimeStampService>();

            _orderServices = new OrderServices(_mockHttpContextAccessor.Object, _mockTokenServices.Object, _mockUnitOfWork.Object, _mockUpdateTimeStampService.Object);
        }


        //*********************************************************************************************************

        [Fact]
        public async Task GetOrder_ShouldCreateOrder_WhenDetailsAreValid()
        {
            //Arrange

            var customerID = "cus_QBjoyy28hBdIYE";

            var user = new User
            {
                userID = "6655a3e32d6ea328db8d8e42",
                firstname = "userf",
                lastname = "userL",
                email = "user@gmail.com",
                strip_CustomerID = customerID,
            };
            var userId = "6655a3e32d6ea328db8d8e42";
            _mockUnitOfWork.Setup(uow => uow.BeginTransactionAsync());
            _mockUnitOfWork.Setup(uow => uow.GetBaseRepository<User>().FirstOrDefaultAsyncByID(customerID, It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(user);


            double orderTotal = 1200;
            string orderSatus = "paid";
            string paymentID = "payment123";
            var SessionID = "session123";

            // Create new order
            var newOrder = new Order
            {
                userID = userId,
                orderDate = DateTime.Now,
                orderTotal = 1200,
                orderSatus = "paid",
                paymentID = "payment123",
                SessionID = "session123"
            };

            // Add new order to database
            _mockUnitOfWork.Setup(uow => uow.GetBaseRepository<Order>().AddAsync(It.IsAny<Order>())).ReturnsAsync(true);
            _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync());

            var user_product = new List<UserCart>
            {
                new UserCart
                {
                    userID = userId,
                    productID = 15,
                    quantity = 2,
                    Product = new Product { productID = 15, price = 100, stock = 20 }
                }
            }.AsQueryable();


            UserCart userProductSample = new UserCart
            {
                userID = userId,
                productID = 15,
                quantity = 2,
                Product = new Product { productID = 15, price = 100, stock = 20 }
            };




            // Get cart items from database
            _mockUnitOfWork.Setup(uw => uw.GetBaseRepository<UserCart>().FindByID(userId, It.IsAny<Expression<Func<UserCart, bool>>>())).Returns(user_product);



            _mockUnitOfWork.Setup(u => u.GetBaseRepository<UserCart>()
                                         .FindByID(It.IsAny<string>(), It.IsAny<Expression<Func<UserCart, bool>>>()))
                           .Returns(user_product.AsQueryable());

            _mockUnitOfWork.Setup(u => u.GetBaseRepository<CurrentOrders>().AddAsync(It.IsAny<CurrentOrders>()));

            _mockUnitOfWork.Setup(u => u.GetBaseRepository<Product>().UpdateColumn(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                           .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.GetBaseRepository<UserCart>().RemoveRangeAsync(It.IsAny<IEnumerable<UserCart>>()))
                           .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.SaveChangesAsync());

            _mockUnitOfWork.Setup(u => u.CommitAsync())
                           .Returns(Task.CompletedTask);
            //Action
            var result = await _orderServices.GetOrder(orderTotal, orderSatus, paymentID, SessionID, customerID);

            //Assert
            Assert.NotEmpty(result.orderSatus);
            Assert.Equal(1200, result.orderTotal);


        }


        //*********************************************************************************************************

        [Fact]
        public async Task GetOrder_ShouldThrowException_WhenCustomerNotFound()
        {
            // Arrange
            string customerID = "CUSTOMER123";
            _mockUnitOfWork.Setup(u => u.GetBaseRepository<User>().FirstOrDefaultAsyncByID(customerID, It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _orderServices.GetOrder(100.0, "Processing", "PAY123", "SESSION123", customerID));
        }



        [Fact]
        public async Task ViewAllOrders_ShouldReturnOrders_WhenOrdersExist()
        {
            // Arrange
            var userID = "6655aff9a6666512e38904c1";
            var orders = new List<Order> { new Order { orderID = 92, userID = userID } };
            _mockUnitOfWork.Setup(u => u.GetBaseRepository<Order>().GetByIDAsync(userID, It.IsAny<Expression<Func<Order, bool>>>()))
                .ReturnsAsync(orders);
            _mockTokenServices.Setup(t => t.GetUserIDClaim())
                .Returns(userID);

            // Act
            var result = await _orderServices.ViewAllOrders();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orders, result);
        }

        [Fact]
        public async Task ViewAllOrders_ShouldReturnNull_WhenUserNotFound()
        {
            // Arrange
            string? userID = null;
            var orders = new List<Order> { new Order { orderID = 92, userID = userID } };
            _mockUnitOfWork.Setup(u => u.GetBaseRepository<Order>().GetByIDAsync(userID, It.IsAny<Expression<Func<Order, bool>>>()))
                .ReturnsAsync(orders);
            _mockTokenServices.Setup(t => t.GetUserIDClaim())
                .Returns(userID);

            // Act
            //var result = await _orderServices.ViewAllOrders();
            var exception = await Assert.ThrowsAsync<Exception>(() => _orderServices.ViewAllOrders());

            // Assert
            Assert.NotNull(exception);
            Assert.Equal("User Id not found!", exception.Message);
        }



        [Fact]
        public async Task ViewAllOrders_ShouldReturnNullResult_WhenResultNotFound()
        {
            // Arrange
            string? userID = "user123";
            List<Order>? orders = null;
            _mockUnitOfWork.Setup(u => u.GetBaseRepository<Order>().GetByIDAsync(userID, It.IsAny<Expression<Func<Order, bool>>>()))
                .ReturnsAsync(orders);
            _mockTokenServices.Setup(t => t.GetUserIDClaim())
                .Returns(userID);

            // Act
            //var result = await _orderServices.ViewAllOrders();
            var exception = await Assert.ThrowsAsync<Exception>(() => _orderServices.ViewAllOrders());

            // Assert
            Assert.NotNull(exception);
            Assert.Equal("Don't have any order found!", exception.Message);
        }

        [Fact]
        public async Task ViewAllOrders_ShouldThrowException_WhenNoOrdersFound()
        {
            // Arrange
            var userID = "USER123";
            _mockUnitOfWork.Setup(u => u.GetBaseRepository<Order>().GetByIDAsync(userID, It.IsAny<Expression<Func<Order, bool>>>()))
                .ThrowsAsync(new Exception("No Order found!"));
            _mockTokenServices.Setup(t => t.GetUserIDClaim())
                .Returns(userID);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _orderServices.ViewAllOrders());
            Assert.Equal("No Order found!", exception.Message);
        }

        [Fact]
        public async Task AddToCart_ShouldAddItemToCart_WhenDetailsAreValid()
        {
            // Arrange
            var cartReq = new UserProductDTO { productID = 14, quantity = 1 };
            var userID = "6655aff9a6666512e38904c1";
            var product = new Product { productID = 15, stock = 10 };

            var newCart = new UserCart
            {
                userID = userID,
                productID = cartReq.productID,
                quantity = cartReq.quantity,
                createDate = DateTime.Now,
                cartCreateDate = DateTime.Now,
                cartUpdateDate = DateTime.Now
            };

            _mockTokenServices.Setup(t => t.GetUserIDClaim())
            .Returns(userID);

            _mockUnitOfWork.Setup(u => u.GetBaseRepository<Product>().FirstOrDefaultAsyncByID(cartReq.productID, It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(product);

            _mockUnitOfWork.Setup(au => au.GetBaseRepository<UserCart>().AddAsync(newCart));


            // _orderServices.updateDateTime(userID);
            //_mockUnitOfWork.Setup(uow => uow.GetBaseRepository<UserCart>().GetByIDAsync(userID, a => a.userID == userID));
            // Act
            var result = await _orderServices.AddToCart(cartReq);

            // Assert
            Assert.Equal("Cart create succesfully!", result);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task AddToCart_ShouldThrowException_WhenProductNotFound()
        {
            // Arrange
            var cartReq = new UserProductDTO { productID = 15, quantity = 1 };
            var userID = "USER123";

            _mockTokenServices.Setup(t => t.GetUserIDClaim())
                .Returns(userID);

            // Mocking the repository to throw an exception for product not found
            _mockUnitOfWork.Setup(u => u.GetBaseRepository<Product>().FirstOrDefaultAsyncByID(cartReq.productID, It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync((Product)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _orderServices.AddToCart(cartReq));
            Assert.Equal("Error occured suring cart creating!", exception.Message);
        }

        [Fact]
        public async Task AddToCart_ShouldThrowException_WhenQuantityExceedsStock()
        {
            // Arrange
            var cartReq = new UserProductDTO { productID = 15, quantity = 11 };
            var userID = "USER123";
            var product = new Product { productID = 15, stock = 10 };

            _mockTokenServices.Setup(t => t.GetUserIDClaim())
                .Returns(userID);
            _mockUnitOfWork.Setup(u => u.GetBaseRepository<Product>().FirstOrDefaultAsyncByID(cartReq.productID, It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(product);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _orderServices.AddToCart(cartReq));
            Assert.Equal("Error occured suring cart creating!", exception.Message);
        }


        //[Fact]
        //public async Task ViewCartItem_ShouldThrowException_WhenNoCartItemsFound()
        //{
        //    // Arrange
        //    string userId = "user123";

        //    List<UserCart> list = null;

        //    var res = list.AsQueryable();
        //    _mockTokenServices.Setup(t => t.GetUserIDClaim()).Returns(userId);

        //    _mockUnitOfWork.Setup(u => u.GetBaseRepository<UserCart>().FindByID(It.IsAny<string>(), It.IsAny<Expression<Func<UserCart, bool>>>()))
        //        .Returns(res);

        //    var service = new UpdateTimeStampService(_mockUnitOfWork.Object);

        //    // Act & Assert
        //    var exception = await Assert.ThrowsAsync<Exception>(() => _orderServices.ViewCartItem());

        //    Assert.Equal("Don't have any cart item!", exception.Message);
        //}



        [Fact]
        public async Task ViewCartItem_ShouldReturnCartItems_WhenCartItemsFound()
        {
            // Arrange
            var userId = "user123";
            var cartItems = new List<UserCart>
        {
            new UserCart
            {
                userID = userId,
                productID = 1,
                quantity = 2,
                createDate = DateTime.Now,
                updateDate = DateTime.Now,
                Product = new Product
                {
                    name = "Product1",
                    price = 100,
                    description = "Description1",
                    imageURL = "http://example.com/image1.jpg",
                    discount = 10,
                    stock = 50
                }
            }
        }.AsQueryable();

            _mockTokenServices.Setup(t => t.GetUserIDClaim()).Returns(userId);
            _mockUnitOfWork.Setup(u => u.GetBaseRepository<UserCart>().FindByID(It.IsAny<string>(), It.IsAny<Expression<Func<UserCart, bool>>>()))
                .Returns(cartItems);

            var service = new UpdateTimeStampService(_mockUnitOfWork.Object);

            // Act
            var result = await _orderServices.ViewCartItem();

            // Assert
            dynamic resultDynamic = result;
            //var user = result.userID;

            Assert.NotNull(result);
            Assert.Equal(userId, resultDynamic.userID);
            Assert.Single(resultDynamic.cartItems);
            Assert.Equal(1, resultDynamic.cartItems[0].productID);
            Assert.Equal(200, resultDynamic.subTotal); // 2 * 100 (quantity * price)
        }

        //[Fact]
        //public async Task ViewCartItem_ShouldReturnNullCartItems_WhenCartItemsNotFound()
        //{
        //    // Arrange
        //    var userId = "user123";
        //    List<UserCart>? cartItems = null;

        //    _mockTokenServices.Setup(t => t.GetUserIDClaim()).Returns(userId);
        //    _mockUnitOfWork.Setup(u => u.GetBaseRepository<UserCart>().FindByID(It.IsAny<string>(), It.IsAny<Expression<Func<UserCart, bool>>>()))
        //        .Returns(cartItems);

        //    var service = new UpdateTimeStampService(_mockUnitOfWork.Object);

        //    // Act
        //    var exceptipn = await Assert.ThrowsAsync<Exception>(() => _orderServices.ViewCartItem());


        //    // Assert
        //    Assert.NotNull(exceptipn);
        //    Assert.Equal("Don't have any cart item!", exceptipn.Message);

        //}


        [Fact]
        public async Task DeleteCartItem_ShouldDeleteItem_WhenItemExists()
        {
            // Arrange
            string userId = "test-user-id";
            int productId = 1;
            var cartItem = new UserCart { productID = productId, userID = userId };

            _mockTokenServices.Setup(t => t.GetUserIDClaim()).Returns(userId);
            _mockUnitOfWork.Setup(r => r.GetBaseRepository<UserCart>().FindByID(productId, userId, It.IsAny<Expression<Func<UserCart, bool>>>()))
                                   .Returns(new List<UserCart> { cartItem }.AsQueryable());
            _mockUnitOfWork.Setup(r => r.GetBaseRepository<UserCart>().RemoveAsync(It.IsAny<UserCart>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync());
            _mockUnitOfWork.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _orderServices.DeleteCartItem(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.productID);
            _mockUnitOfWork.Verify(r => r.GetBaseRepository<UserCart>().RemoveAsync(It.IsAny<UserCart>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteCartItem_ShouldThrowException_WhenItemNotFound()
        {
            // Arrange
            string userId = "test-user-id";
            int productId = 1;

            _mockTokenServices.Setup(t => t.GetUserIDClaim()).Returns(userId);
            _mockUnitOfWork.Setup(r => r.GetBaseRepository<UserCart>().FindByID(productId, userId, It.IsAny<Expression<Func<UserCart, bool>>>()))
                                   .Returns(new List<UserCart>().AsQueryable());
            _mockUnitOfWork.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _orderServices.DeleteCartItem(productId));
            Assert.Equal("Error occured during delete ", exception.Message);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteCartItem_ShouldThrowException_WhenErrorOccursDuringDelete()
        {
            // Arrange
            string userId = "test-user-id";
            int productId = 1;
            var cartItem = new UserCart { productID = productId, userID = userId };

            _mockTokenServices.Setup(t => t.GetUserIDClaim()).Returns(userId);
            _mockUnitOfWork.Setup(r => r.GetBaseRepository<UserCart>().FindByID(productId, userId, It.IsAny<Expression<Func<UserCart, bool>>>()))
                                   .Returns(new List<UserCart> { cartItem }.AsQueryable());
            _mockUnitOfWork.Setup(r => r.GetBaseRepository<UserCart>().RemoveAsync(It.IsAny<UserCart>())).Throws(new Exception("Database error"));
            _mockUnitOfWork.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _orderServices.DeleteCartItem(productId));
            Assert.Equal("Error occured during delete ", exception.Message);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
        }
    }
}
