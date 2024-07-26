using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using ShoppingCart.Application.DTOs;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Application.Interfaces.IRepositories;
using ShoppingCart.Application.Services.Implementation;
using ShoppingCart.Domain.Models;
using Xunit;

namespace ShoppingCartAPI.Test.Tests.ServicesTesting
{
    public class AddressServiceTests
    {
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<ITokenServices> _mockTokenServices;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly AddressService _addressService;

        public AddressServiceTests()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockTokenServices = new Mock<ITokenServices>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _addressService = new AddressService(
                _mockHttpContextAccessor.Object,
                _mockTokenServices.Object,
                _mockUnitOfWork.Object);

            // Setup HttpContext
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "USER123") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = claimsPrincipal };
            _mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);

            // Setup token service
            _mockTokenServices.Setup(ts => ts.GetUserIDClaim()).Returns("USER123");
        }

        [Fact]
        public async Task AddAddress_ShouldAddNewAddress_WhenDetailsAreValid()
        {
            // Arrange
            var addressReq = new AddressDTO
            {
                street = "123 Main St",
                city = "Test City",
                country = "Test Country",
                postalCode = "12345"
            };

            _mockUnitOfWork.Setup(uow => uow.GetBaseRepository<Address>().AddAsync(It.IsAny<Address>()))
                .Returns(Task.FromResult(true));
            _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync()).ReturnsAsync(1);
            _mockUnitOfWork.Setup(uow => uow.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(uow => uow.RollbackAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _addressService.AddAddress(addressReq);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("USER123", result.userID);
            Assert.Equal(addressReq.street, result.street);
            Assert.Equal(addressReq.city, result.city);
            Assert.Equal(addressReq.country, result.country);
            Assert.Equal(addressReq.postalCode, result.postalCode);
            _mockUnitOfWork.Verify(uow => uow.GetBaseRepository<Address>().AddAsync(It.IsAny<Address>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.AtLeastOnce);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        }



        [Fact]
        public async Task AddAddress_ShouldThrowException_WhenUserIDIsInvalid()
        {
            // Arrange
            var addressReq = new AddressDTO
            {
                street = "123 Main St",
                city = "Test City",
                country = "Test Country",
                postalCode = "12345"
            };

            _mockTokenServices.Setup(ts => ts.GetUserIDClaim()).Returns(string.Empty);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _addressService.AddAddress(addressReq));
            _mockUnitOfWork.Verify(uow => uow.GetBaseRepository<Address>().AddAsync(It.IsAny<Address>()), Times.Never);
            _mockUnitOfWork.Verify(uow => uow.RollbackAsync(), Times.Once);
        }



        [Fact]
        public async Task AddAddress_ShouldRollbackTransaction_WhenExceptionIsThrown()
        {
            // Arrange
            var addressReq = new AddressDTO
            {
                street = "123 Main St",
                city = "Test City",
                country = "Test Country",
                postalCode = "12345"
            };

            _mockUnitOfWork.Setup(uow => uow.GetBaseRepository<Address>().AddAsync(It.IsAny<Address>()))
                .Throws(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _addressService.AddAddress(addressReq));
            _mockUnitOfWork.Verify(uow => uow.RollbackAsync(), Times.Once);
        }



        [Fact]
        public async Task ViewAddress_ShouldReturnListOfAddresses_WhenUserHasAddresses()
        {
            // Arrange
            var addresses = new List<Address>
            {
                new Address { userID = "USER123", street = "123 Main St", city = "Test City", country = "Test Country", postalCode = "12345" }
            }.AsQueryable();

            var userID = "USER123";

            _mockUnitOfWork.Setup(uow => uow.GetBaseRepository<Address>().GetByIDAsync(userID, It.IsAny<Expression<Func<Address, bool>>>())).ReturnsAsync(addresses.ToList());

            // Act
            var result = await _addressService.ViewAddress();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("USER123", result.First().userID);
            _mockUnitOfWork.Verify(uow => uow.GetBaseRepository<Address>().GetByIDAsync(userID, It.IsAny<Expression<Func<Address, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task ViewAddress_ShouldThrowException_WhenAddressesNotFound()
        {
            // Arrange
            var addresses = new List<Address>().AsQueryable();

            var userID = "USER123";

            _mockUnitOfWork.Setup(uow => uow.GetBaseRepository<Address>().GetByIDAsync(userID, It.IsAny<Expression<Func<Address, bool>>>())).ReturnsAsync(addresses.ToList());

            // Act
            var result = await _addressService.ViewAddress();

            // Assert
            Assert.Empty(result);
        }
    }
}
