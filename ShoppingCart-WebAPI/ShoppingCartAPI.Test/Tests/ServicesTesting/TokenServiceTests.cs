using Microsoft.AspNetCore.Http;
using Moq;
using ShoppingCart.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartAPI.Test.Tests.ServicesTesting
{
    public class TokenServiceTests
    {
        private readonly TokenService _tokenService;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        public TokenServiceTests()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _tokenService = new TokenService(_mockHttpContextAccessor.Object);
        }

        [Fact]
        public void GetUserIDClaim_ShouldReturnUserID()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "auth0|123456")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext { User = principal };

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            // Act
            var result = _tokenService.GetUserIDClaim();

            // Assert
            Assert.Equal("123456", result);
        }

        [Fact]
        public void GetUserIDClaim_ShouldThrowException_WhenClaimNotFound()
        {
            // Arrange
            var claims = new List<Claim>();
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext { User = principal };

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _tokenService.GetUserIDClaim());
            Assert.Equal("The user ID claim was not found.", exception.Message);
        }

        [Fact]
        public void GetUserIDClaim_ShouldThrowException_WhenClaimFormatIsInvalid()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "invalidformat")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext { User = principal };

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _tokenService.GetUserIDClaim());
            Assert.Equal("The user ID claim is not in the expected format.", exception.Message);
        }
    }
}
