//using Blazored.LocalStorage;
//using Blazored.SessionStorage;
//using Bunit;
//using Moq;
//using Bunit.TestDoubles;
//using Microsoft.Extensions.DependencyInjection;
//using ShoppingApp.FrontEnd.Interfaces;
//using ShoppingApp.FrontEnd.Pages;
//using ShoppingApp.FrontEnd.Services;
//using ShoppingApp.FrontEnd.Models;
//using Microsoft.AspNetCore.Components;

//namespace ShoppingApp.FrontEnd.Test.Page_Testing
//{
//    public class LoginPageTests : TestContext
//    {
//        private readonly IUserServices _userServiceMock;
//        public LoginPageTests()
//        {
//            Services.AddBlazorBootstrap();
//            Services.AddBlazoredLocalStorage();
//            Services.AddBlazoredSessionStorage();
//            _userServiceMock = new MockUserService();
//            Services.AddSingleton<IUserServices>(_userServiceMock);
//            //Services.AddScoped<IUserServices, UserServices>();
//        }
//        [Fact]
//        public void LoginPageRendersCorrectly()
//        {
//            // Arrange
//            var cut = RenderComponent<Login>();


//            // Assert
//            cut.MarkupMatches(@"<link rel=""stylesheet"" href=""/css/Login.css"">
//                                <nav aria-label=""breadcrumb"">
//                                  <ol class=""breadcrumb"">
//                                    <li class=""breadcrumb-item"">
//                                      <a href=""/"">Home</a>
//                                    </li>
//                                    <li class=""breadcrumb-item active"" aria-current=""page"">Login</li>
//                                  </ol>
//                                </nav>
//                                <div class=""login-main-container"">
//                                  <div class=""image-section"">
//                                    <img src=""https://img.freepik.com/free-photo/computer-security-with-login-password-padlock_107791-16191.jpg?w=740&amp;t=st=1719208128~exp=1719208728~hmac=58c411f12166d7409201b29dc76074b76d29c9639221ac8a212f3ad6f9681fda"">
//                                  </div>
//                                  <div class=""login-sub-container"">
//                                    <div class=""header-section"">
//                                      <h1>Sign In</h1>
//                                      <p>Welcome Back! Let's continue where you left off.</p>
//                                    </div>
//                                    <div class=""main-container"">
//                                      <form class=""login-form"" >
//                                        <div class=""form-group"">
//                                          <input type=""email"" class=""form-control"" id=""Email1"" aria-describedby=""emailHelp"" placeholder=""Enter email"" >
//                                          <small id=""emailHelp"" class=""form-text text-muted"">We'll never share your email with anyone else.</small>
//                                        </div>
//                                        <div class=""form-group"">
//                                          <input type=""password"" class=""form-control"" id=""Password1"" placeholder=""Enter password"" >
//                                        </div>
//                                        <button type=""submit"" class=""btn btn-primary"">SignIn</button>
//                                        <small class=""signup-messsage"">If you don't have an account, please
//                                          <a href=""signup"">Signup</a>
//                                          here</small>
//                                      </form>
//                                    </div>
//                                  </div>
//                                </div>");
//        }


//        [Fact]
//        public async Task Login_Successful_Redirects_To_Products()
//        {
//            // Arrange
//            var loginComponent = RenderComponent<Pages.Login>();

//            // Simulate user entering valid login credentials
//            var emailInput = loginComponent.Find("#Email1");
//            emailInput.Change("test@example.com");

//            var passwordInput = loginComponent.Find("#Password1");
//            passwordInput.Change("password");

//            // Act
//            loginComponent.Find("form").Submit();

//            // Wait for asynchronous operations to complete
//            await loginComponent.InvokeAsync(() => Task.Delay(1000));

//            // Assert
//            //Assert.Contains("/products", NavigationManager.Uri);
//            //Assert.Equal("mock_access_token", _userServiceMock.LastAccessToken); // Verify access token set
//        }



//        [Fact]
//        public async Task SuccessfulLogin()
//        {
//            // Arrange
//            var mockUserService = new Mock<IUserServices>();
//            mockUserService.Setup(service => service.GetAccessToken(It.IsAny<LoginModel>()))
//                           .ReturnsAsync(new TokenModel { Access_Token = "valid_token", Refresh_Token = "refresh_token" });

//            Services.AddSingleton(mockUserService.Object);

//            var cut = RenderComponent<Login>();

//            // Act
//            cut.Find("#Email1").Change("test@example.com");
//            cut.Find("#Password1").Change("password");
//            cut.Find("form").SubmitAsync();

//            // Assert
//            //mockUserService.Verify(service => service.GetAccessToken(It.IsAny<LoginModel>()), Times.Once);
//            Assert.Equal("valid_token", cut.Instance.accessToken);
//        }
//    }


//    // Mock implementation of IUserServices for testing purposes
//    public class MockUserService : IUserServices
//    {
//        public string LastAccessToken { get; private set; }

//        public async Task<TokenModel> GetAccessToken(LoginModel loginData)
//        {
//            // Simulate a successful login
//            if (loginData.email == "test@example.com" && loginData.password == "password")
//            {
//                await Task.Delay(1000); // Simulate delay as in real service
//                LastAccessToken = "mock_access_token";
//                return new TokenModel
//                {
//                    Access_Token = "mock_access_token",
//                    Refresh_Token = "mock_refresh_token"
//                };
//            }
//            else
//            {
//                await Task.Delay(1000); // Simulate delay as in real service
//                throw new Exception("Invalid credentials");
//            }
//        }


//        Task<bool> IUserServices.RegisterUser(UserModel user)
//        {
//            throw new NotImplementedException();
//        }

//        Task<List<AddressModel>> IUserServices.UserAddressInfo()
//        {
//            throw new NotImplementedException();
//        }

//        Task<UserModel> IUserServices.UserInfo()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
