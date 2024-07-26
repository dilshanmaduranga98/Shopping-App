using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ShoppingCart.Application.Interfaces.IRepositories;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Infrastructure.AuthServicec;
using ShoppingCart.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using ShoppingCart.Domain.Models;
using Stripe;
using Product = ShoppingCart.Domain.Models.Product;
using System.Linq.Expressions;

namespace ShoppingCartAPI.Test.Tests.ServicesTesting
{
    public class StripeServiceTest
    {
        private readonly Mock<ITokenServices> _tokenServicesMock;
        private readonly Mock<IHttpContextAccessor> _contextAccessorMock;
        private readonly Mock<ILogger<PaymentService>> _loggerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IOptions<KeyConfigurations>> _optionsMock;
        private readonly PaymentService _paymentService;

        public StripeServiceTest()
        {
            _tokenServicesMock = new Mock<ITokenServices>();
            _contextAccessorMock = new Mock<IHttpContextAccessor>();
            _loggerMock = new Mock<ILogger<PaymentService>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _optionsMock = new Mock<IOptions<KeyConfigurations>>();
            _optionsMock.Setup(o => o.Value).Returns(new KeyConfigurations { StripeKey = "sk_test_51PB819EeniyGZUeWfyjxE1z9kqiRUnq5BsDJlB2CJMB8Els89QZrm7RqAqhcrXjyYQDNHySCaFTKcriEAD6NdQNv005Pu0MDCy" });

            _paymentService = new PaymentService(
                _tokenServicesMock.Object,
                _contextAccessorMock.Object,
                _loggerMock.Object,
                _optionsMock.Object,
                _unitOfWorkMock.Object
            );
        }
        [Fact]
        public async Task CreateCustomer_ShouldReturnCustomer()
        {
            // Arrange
            var customerServiceMock = new Mock<CustomerService>();
            var customer = new Customer { Id = "cus_QI5iGGbxnRx9lr", Email = "test@example.com", Name = "Test User" };
            var customerStripe = new CustomerCreateOptions { Email = "test@example.com", Name = "Test User" };
            // customerServiceMock.Setup(s => s.CreateAsync(customerStripe)).ReturnsAsync(customer);

            //StripeConfiguration.ApiKey = "sk_test_51PB819EeniyGZUeWfyjxE1z9kqiRUnq5BsDJlB2CJMB8Els89QZrm7RqAqhcrXjyYQDNHySCaFTKcriEAD6NdQNv005Pu0MDCy";

            // Act
            var result = await _paymentService.CreateCustomer("Test User", "test@example.com");

            // Assert
           result.Should().NotBeNull(customer.ToString());
            Assert.Equal("test@example.com", result.Email);
            Assert.Equal("Test User", result.Name);
        }

        [Fact]
        public async Task ItemCheckout_ShouldReturnSessionUrl()
        {
            // Arrange

            StripeConfiguration.ApiKey = "sk_test_51PB819EeniyGZUeWfyjxE1z9kqiRUnq5BsDJlB2CJMB8Els89QZrm7RqAqhcrXjyYQDNHySCaFTKcriEAD6NdQNv005Pu0MDCy";
            var userId = "user_123";
            _tokenServicesMock.Setup(t => t.GetUserIDClaim()).Returns(userId);

            var user = new User { userID = userId, strip_CustomerID = "cus_QBjoyy28hBdIYE" };
            var userCart = new List<UserCart>
        {
            new UserCart { userID = userId, quantity = 1, Product = new Product { name = "Product 1", price = 100, stock = 10 } },
            new UserCart { userID = userId,quantity = 2, Product = new Product { name = "Product 2", price = 200, stock = 20 } }
        }.AsQueryable();

            //var userRepositoryMock = new Mock<IBaseRepository<User>>();
            _unitOfWorkMock.Setup(r => r.GetBaseRepository<User>().FirstOrDefaultAsyncByID(userId, It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(user);
            //_unitOfWorkMock.Setup(u => u.GetBaseRepository<User>()).Returns(userRepositoryMock.Object);

            //var userCartRepositoryMock = new Mock<IBaseRepository<UserCart>>();
            _unitOfWorkMock.Setup(r => r.GetBaseRepository<UserCart>().FindByID(userId, It.IsAny<Expression<Func<UserCart, bool>>>())).Returns(userCart);
            //_unitOfWorkMock.Setup(u => u.GetBaseRepository<UserCart>()).Returns(userCartRepositoryMock.Object);

            

            // Act
            var result = await _paymentService.ItemCheckout();

            // Assert
            result.Should().Contain("https://");
        }

        [Fact]
        public async Task PaymentHistory_ShouldReturnPaymentHistory()
        {
            // Arrange
            var userId = "user_123";
            _tokenServicesMock.Setup(t => t.GetUserIDClaim()).Returns(userId);

            var payments = new List<Order>
        {
            new Order { orderID = 1, orderDate = DateTime.Now, orderSatus = "Completed", orderTotal = 1000 },
            new Order { orderID = 2, orderDate = DateTime.Now.AddDays(-1), orderSatus = "Completed", orderTotal = 2000 }
        }.AsQueryable();

            //var orderRepositoryMock = new Mock<IBaseRepository<Order>>();
            _unitOfWorkMock.Setup(r => r.GetBaseRepository<Order>().FindByID(userId, It.IsAny<Expression<Func<Order, bool>>>())).Returns(payments);
           // _unitOfWorkMock.Setup(u => u.GetBaseRepository<Order>()).Returns(orderRepositoryMock.Object);

            // Act
            var result = await _paymentService.PaymentHistory();

            // Assert
            result.Should().HaveCount(2);
            Assert.Equal("Completed", result[0].order_status);
            Assert.Equal(1000, result[0].total);
            Assert.Equal(1, result[0].order_id);
            Assert.Equal(2, result[1].order_id);
        }

        [Fact]
        public async Task PaymentHistoryProducts_ShouldReturnProducts()
        {
            // Arrange
            var orderId = 1;
            var userId = "user_123";
            _tokenServicesMock.Setup(t => t.GetUserIDClaim()).Returns(userId);


            //var orders = new List<Order>();
            var order = new List<Order>
            {
                new Order
                {
                    orderID = orderId,
                    CurrentOrders = new List<CurrentOrders>
                    {
                        new CurrentOrders { Product = new Product { name = "Product 1", price = 100 }, orderQuantity = 1 },
                        new CurrentOrders { Product = new Product { name = "Product 2", price = 200 }, orderQuantity = 2 }
                    }
                }
            };

            //var orderRepositoryMock = new Mock<IBaseRepository<Order>>();
            _unitOfWorkMock.Setup(r => r.GetBaseRepository<Order>().FindByID(It.IsAny<int>(), It.IsAny<Expression<Func<Order, bool>>>()))
                .Returns(order.AsQueryable());
            _unitOfWorkMock.Setup(r => r.GetBaseRepository<Order>().FindByID(It.IsAny<int>(), It.IsAny<Expression<Func<Order, bool>>>()))
                .Returns(order.AsQueryable());
            //_unitOfWorkMock.Setup(u => u.GetBaseRepository<Order>()).Returns(orderRepositoryMock.Object);

            // Act
            var result = await _paymentService.PaymentHistoryProducts(orderId);

            // Assert
            Assert.Equal(2, result.Count());

            Assert.Equal("Product 1", result[0].name);
            Assert.Equal(100, result[0].price);
            Assert.Equal(1, result[0].quantity);

            Assert.Equal("Product 2", result[1].name);
            Assert.Equal(200, result[1].price);
            Assert.Equal(2, result[1].quantity);
        }
    }
}
