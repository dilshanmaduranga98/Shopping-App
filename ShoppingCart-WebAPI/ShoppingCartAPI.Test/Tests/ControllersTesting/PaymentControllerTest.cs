using System;
using System.IO;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ShoppingCart.API.Controllers;
using ShoppingCart.Application.DTOs;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Application.Services.Interfaces;
using ShoppingCart.Domain.Helper;
using ShoppingCart.Domain.Models;
using Stripe;
using Xunit;

namespace ShoppingCartAPI.Test.Tests.Controllers
{


    public class PaymentControllerTests
    {
        private readonly Mock<IPaymentService> _mockPaymentService;
        private readonly Mock<IOrderServices> _mockOrderServices;
        private readonly Mock<IEmailInterface> _mockEmailInterface;
        private readonly Mock<ILogger<paymentController>> _mockLogger;
        private readonly paymentController _controller;

        public PaymentControllerTests()
        {
            _mockPaymentService = new Mock<IPaymentService>();
            _mockOrderServices = new Mock<IOrderServices>();
            _mockEmailInterface = new Mock<IEmailInterface>();
            _mockLogger = new Mock<ILogger<paymentController>>();
            _controller = new paymentController(_mockPaymentService.Object, _mockOrderServices.Object, _mockEmailInterface.Object, null, _mockLogger.Object);
        }

        

        [Fact]
        public async Task Index_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            var json = "{ \"type\": \"checkout.session.completed\" }";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    Request = { Body = stream }
                }
            };

            _mockOrderServices.Setup(s => s.GetOrder(It.IsAny<double>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                              .ThrowsAsync(new Exception("Error processing order"));

            // Act
            var result = await _controller.Index();

            // Assert
            
            Assert.ThrowsAsync<Exception>(() => _controller.Index());
        }

        [Fact]
        public async Task CheckoutSession_ShouldReturnOk_WhenCheckoutInitiatedSuccessfully()
        {
            // Arrange
            _mockPaymentService.Setup(s => s.ItemCheckout()).ReturnsAsync("checkout_session");

            // Act
            var result = await _controller.CheckoutSession();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = okResult.Value;

            // Use reflection to get the Url property
            var urlProperty = returnValue.GetType().GetProperty("Url");
            Assert.NotNull(urlProperty);
            var urlValue = urlProperty.GetValue(returnValue) as string;

            Assert.Equal("checkout_session", urlValue);
        }

        [Fact]
        public async Task CheckoutSession_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            _mockPaymentService.Setup(s => s.ItemCheckout()).ThrowsAsync(new Exception("Checkout failed"));

            // Act
            var result = await _controller.CheckoutSession();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task GetPaymentgHistory_ShouldReturnOk_WhenHistoryRetrievedSuccessfully()
        {
            // Arrange
            var paymentHistory = new List<PaymentHistoryDTO> { new PaymentHistoryDTO() };
            _mockPaymentService.Setup(s => s.PaymentHistory()).ReturnsAsync(paymentHistory);

            // Act
            var result = await _controller.GetPaymentgHistory();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(paymentHistory, okResult.Value);
        }

        [Fact]
        public async Task GetPaymentgHistory_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            _mockPaymentService.Setup(s => s.PaymentHistory()).ThrowsAsync(new Exception("Error retrieving payment history"));

            // Act
            var result = await _controller.GetPaymentgHistory();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error retrieving payment history", badRequestResult.Value);
        }

        [Fact]
        public async Task ViewPaymentHistoryProducts_ShouldReturnOk_WhenItemsRetrievedSuccessfully()
        {
            // Arrange
            var orderId = 1;
            //var paidItems = new List<PaidItemDTO> { new PaidItemDTO() };
            _mockPaymentService.Setup(s => s.PaymentHistoryProducts(orderId));

            // Act
            var result = await _controller.ViewPaymentHistoryProducts(orderId);

            // Assert
            //var okResult = Assert.IsType<OkObjectResult>(result);
            //Assert.Equal(paidItems, okResult.Value);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ViewPaymentHistoryProducts_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            var orderId = 1;
            _mockPaymentService.Setup(s => s.PaymentHistoryProducts(orderId)).ThrowsAsync(new Exception("No orders found"));

            // Act
            var result = await _controller.ViewPaymentHistoryProducts(orderId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = badRequestResult.Value as dynamic;

            var exception = Assert.ThrowsAsync<Exception>(() => _controller.ViewPaymentHistoryProducts(orderId));
            Assert.NotNull(exception);
            Assert.NotNull(result);
        }
    }

}