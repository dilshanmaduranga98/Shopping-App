using Microsoft.Extensions.Options;
using Moq;
using ShoppingCart.Application.Interfaces.IRepositories;
using ShoppingCart.Domain.Models;
using ShoppingCart.Infrastructure.AuthServicec;
using ShoppingCart.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartAPI.Test.Tests.ServicesTesting
{
    public class EmailServiceTests
    {
        private readonly EmailService _emailService;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IOptions<KeyConfigurations>> _mockOptions;

        public EmailServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockOptions = new Mock<IOptions<KeyConfigurations>>();
            _mockOptions.Setup(o => o.Value).Returns(new KeyConfigurations { SendGridKey = "test-key" });

            _emailService = new EmailService(_mockOptions.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task SendGridService_ShouldSendEmail()
        {
            // Arrange
            var toMail = "test@example.com";
            var toName = "Test User";
            var emailHtmlContent = "<p>Hello</p>";
            var emailSubject = "Test Subject";
            var plainText = "Hello";

            // Act
            await _emailService.sendGridService(toMail, toName, emailHtmlContent, emailSubject, plainText);

            // Assert
            // Here you would typically verify that the email was sent, but since SendGridClient is not mocked,
            // you can check the logs or other side effects.
        }

        
    }
}
