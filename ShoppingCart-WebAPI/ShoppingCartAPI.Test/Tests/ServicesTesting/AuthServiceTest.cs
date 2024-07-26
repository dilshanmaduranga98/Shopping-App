using Azure.Core;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using ShoppingCart.Application.DTOs;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Application.Interfaces.IRepositories;
using ShoppingCart.Domain.Models;
using ShoppingCart.Infrastructure.AuthServicec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartAPI.Test.Tests.ServicesTesting
{
    public class AuthServiceTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IPaymentService> _mockPaymentService;
        private readonly Mock<IEmailInterface> _mockMailInterface;
        private readonly Mock<IOptions<KeyConfigurations>> _mockKeyConfigurations;
        private readonly AuthService _authService;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        //private readonly 

        public AuthServiceTest()
        {
            _mockKeyConfigurations = new Mock<IOptions<KeyConfigurations>>();
            _mockMailInterface = new Mock<IEmailInterface>();
            _mockPaymentService = new Mock<IPaymentService>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();


            var config = new KeyConfigurations
            {
                ClientId = "test_client_id",
            };


            _mockKeyConfigurations.Setup(x => x.Value).Returns(config);

            _authService = new AuthService(
                _mockKeyConfigurations.Object,
                _mockPaymentService.Object,
                _mockMailInterface.Object,
                _mockUnitOfWork.Object);

        }


        [Fact]
        public async Task RegisterUser_shouldReturnSuccessCode_whenUserRegisterSuccess()
        {
            // Arrange
            var userreq = new UserDTO
            {
                userID = "user123",
                email = "user@gmail.com",
                phoneNumber = 111111111,
                firstName = " user",
                LastName = "lastUser",
                password = "password",
                confirmPassword = "password",
                addressDTO = new AddressDTO
                {
                    street = "testStreet",
                    city = "city",
                    country = "testCountry",
                    postalCode = "1200",

                }

            };

           // _mockUnitOfWork.Setup(uow => uow.BeginTransactionAsync());

            var auth0Response = new TokenID { _id = "auth0|123456" };
            var auth0ResponseJson = JsonConvert.SerializeObject(auth0Response);

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri == new Uri("https://dev-j510p5iw1blv70i3.us.auth0.com/dbconnections/signup")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(auth0ResponseJson, Encoding.UTF8, "application/json")
                });

            var client = new HttpClient(handlerMock.Object);
            _authService.SetHttpClient(client);


            _mockPaymentService.Setup(s => s.CreateCustomer(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new Stripe.Customer { Id = "cust_12345" });

            _mockUnitOfWork.Setup(t => t.GetBaseRepository<User>().AddAsync(It.IsAny<User>()))
                .ReturnsAsync(true);

            _mockUnitOfWork.Setup(t => t.GetBaseRepository<Address>().AddAsync(It.IsAny<Address>()))
                .ReturnsAsync(true);

            // Act
            var result = await _authService.userSignup(userreq);

            // Assert
            Assert.Equal("User create succefully!", result);
        }


        [Fact]
        public async Task RegiterUser_ReturnBadRequest_WhenUserDetailsNotValid()
        {
            // Arrange
            var userreq = new UserDTO
            {
                userID = "user123",
                email = "gmail.com",
                phoneNumber = 111111111,
                firstName = " user",
                LastName = "lastUser",
                password = "password",
                confirmPassword = "password",
                addressDTO = new AddressDTO
                {
                    street = "testStreet",
                    city = "city",
                    country = "testCountry",
                    postalCode = "1200",

                }

            };

            var auth0Response = new TokenID { _id = "auth0|123456" };
            var auth0ResponseJson = JsonConvert.SerializeObject(auth0Response);

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri == new Uri("https://dev-j510p5iw1blv70i3.us.auth0.com/dbconnections/signup")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(auth0ResponseJson, Encoding.UTF8, "application/json")
                });

            var client = new HttpClient(handlerMock.Object);
            _authService.SetHttpClient(client);


            _mockPaymentService.Setup(s => s.CreateCustomer(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new Stripe.Customer { Id = "cust_12345" });

            _mockUnitOfWork.Setup(t => t.GetBaseRepository<User>().AddAsync(It.IsAny<User>()))
                .ReturnsAsync(true);

            _mockUnitOfWork.Setup(t => t.GetBaseRepository<Address>().AddAsync(It.IsAny<Address>()))
                .ReturnsAsync(true);

            // Act
            var result = await _authService.userSignup(userreq);

            // Assert
            Assert.Equal("Response not give sucess code 200!", result);
        }


        [Fact]
        public async Task UserLogin_WhenGiveSuccessCode_UserCredentialsAreValid()
        {
            // Arrange

            string email = "test@gmail.com";
            string password = "password";

            var auth0Response = new AccessTokenDTO { access_token = "access-token-123" };
            var accessToken = JsonConvert.SerializeObject(auth0Response);

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri == new Uri("https://dev-j510p5iw1blv70i3.us.auth0.com/oauth/token")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(accessToken, Encoding.UTF8, "application/json")
                });

            var client = new HttpClient(handlerMock.Object);
            _authService.SetHttpClient(client);

            var accesTOken = "access-token-123";
          

            // Act
            var result = await _authService.userLogin(email, password);

            // Assert
            Assert.Equal("access-token-123", result.access_token);
        }



        [Fact]
        public async Task UserLogin_ReturnErrore_WhenAccesTokenNotRecived()
        {
            // Arrange

            string email = "gmail.com";
            string password = "password";

            var auth0Response = new AccessTokenDTO { access_token = "access-token-123" };
            var accessToken = JsonConvert.SerializeObject(auth0Response);

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri == new Uri("https://dev-j510p5iw1blv70i3.us.auth0.com/oauth/token")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(accessToken, Encoding.UTF8, "application/json")
                });

            var client = new HttpClient(handlerMock.Object);
            _authService.SetHttpClient(client);

            var expectedResult = new AccessTokenDTO();


            // Act
            var result = await _authService.userLogin(email, password);

            // Assert
            Assert.Equal(expectedResult.access_token, result.access_token);
            Assert.Equal(expectedResult.refresh_token, result.refresh_token);
        }


        [Fact]
        public async Task GetAccessTokenFromRefreshToken_ReturnsNull_OnUnSuccessful()
        {
            // Arrange
            var token = "test-refresh-token";
            var expectedResponse = new AccessTokenDTO();
            var responseContent = JsonConvert.SerializeObject(expectedResponse);

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri == new Uri("https://dev-j510p5iw1blv70i3.us.auth0.com/oauth/token")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _authService.GetAccessTokenFromRefreshToken(token);

            // Assert
           // Assert.NotNull(result);
            Assert.Equal(expectedResponse.access_token, result.access_token);
           
        }
    }
}
