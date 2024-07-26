using Moq;
using ShoppingCart.Application.Interfaces.IRepositories;
using ShoppingCart.Application.Services.Implementation;
using ShoppingCart.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace ShoppingCartAPI.Test.Tests.ServicesTesting
{
    public class UpdateTimeStampServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IBaseRepository<UserCart>> _mockUserCartRepository;
        private readonly UpdateTimeStampService _updateTimeStampService;

        public UpdateTimeStampServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserCartRepository = new Mock<IBaseRepository<UserCart>>();

            _mockUnitOfWork.Setup(u => u.GetBaseRepository<UserCart>()).Returns(_mockUserCartRepository.Object);

            _updateTimeStampService = new UpdateTimeStampService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task UpdateDateTime_UpdatesCartItemsAndSavesChanges()
        {
            // Arrange
            var userId = "test-user-id";
            var userCarts = new List<UserCart>
            {
                new UserCart { userID = userId, cartUpdateDate = DateTime.Now.AddDays(-1) },
                new UserCart { userID = userId, cartUpdateDate = DateTime.Now.AddDays(-1) }
            };

            _mockUserCartRepository
                .Setup(repo => repo.GetByIDAsync(userId, It.IsAny<Expression<Func<UserCart, bool>>>()))
                .ReturnsAsync(userCarts);

            // Act
            var result = await _updateTimeStampService.updateDateTime(userId);

            // Assert
            Assert.Equal("update time!", result);

            foreach (var cart in userCarts)
            {
                Assert.NotNull(cart.cartUpdateDate);
                Assert.Equal(DateTime.Now.Date, cart.cartUpdateDate.Value.Date);
            }

            _mockUserCartRepository.Verify(repo => repo.GetByIDAsync(userId, It.IsAny<Expression<Func<UserCart, bool>>>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
