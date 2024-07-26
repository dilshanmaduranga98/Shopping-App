using Bunit;
using Xunit;
using ShoppingApp.FrontEnd.Pages;
using Microsoft.Extensions.DependencyInjection;
using ShoppingApp.FrontEnd.Test.StubMoqServices;
using ShoppingApp.FrontEnd.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Blazored.LocalStorage;
using ShoppingApp.FrontEnd.Services;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Moq;
using ShoppingApp.FrontEnd.Authentication;

namespace ShoppingApp.FrontEnd.Test.Page_Testing
{
    
    public class HomePageTest : TestContext
    {
        public HomePageTest()
        {
            Services.AddAuthorizationCore();
            Services.AddSingleton<IAuthorizationPolicyProvider, DefaultAuthorizationPolicyProvider>();
            Services.AddBlazorBootstrap();
            Services.AddBlazoredLocalStorage();
            Services.AddBlazoredSessionStorage();
            Services.AddScoped<IOrderServices, OrderServices>();
            Services.AddScoped<IProdcutServices, ProductServiceMoq>();
            this.AddTestAuthorization();
            Services.AddScoped<AuthenticationStateProvider, MockCustomAuthenticationStateProvider>(); // Use the mock directly

        }



        [Fact]
        public void HomePage_defaultState_ExpectedTitle()
        {

            var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
            Services.AddSingleton<Task<AuthenticationState>>(authState);


            var stubProductServices = new ProductServiceMoq();
            Services.AddSingleton<IProdcutServices>(stubProductServices);

            var renderComponent = RenderComponent<Home>(parameters => parameters
                .AddCascadingValue(authState));
            renderComponent
                .Find("h1")
                .MarkupMatches(" <h1>Upgrade your closet today</h1>");
        }



        [Fact]
        public void HomePage_DefaultState_ExpectedLoginButton_WithOutAuthrized()
        {
            var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
            Services.AddSingleton<Task<AuthenticationState>>(authState);


            var stubProductServices = new ProductServiceMoq();
            Services.AddSingleton<IProdcutServices>(stubProductServices);

            var renderComponent = RenderComponent<Home>(parameters => parameters
                .AddCascadingValue(authState));
            renderComponent
                .Find("a")
                .MarkupMatches("  <a href=\"/login\" role=\"button\" class=\"explor-button\">Explore more</a>");
        }

        [Fact]
        public void HomePage_DefaultState_ExpectedProductButton_WithInAuthrized()
        {
            // Arrange
            var stubProductServices = new ProductServiceMoq();
            Services.AddSingleton<IProdcutServices>(stubProductServices);

            var authContext = this.AddTestAuthorization();
            authContext.SetAuthorized("TestUser");


            var renderComponent = RenderComponent<Home>();

            // Assert
            renderComponent
                .Find("a[role='button'].explor-button")
                .MarkupMatches(@"<a href=""/products"" role=""button"" class=""explor-button"">Explore more</a>");
        }

        [Fact]
        public void HomePage_DefaultState_ExpectedProductCategoryButton_InCategorySection_WithInAuthrized()
        {
            // Arrange
            var stubProductServices = new ProductServiceMoq();
            Services.AddSingleton<IProdcutServices>(stubProductServices);

            var authContext = this.AddTestAuthorization();
            authContext.SetAuthorized("TestUser");


            var renderComponent = RenderComponent<Home>();

            // Assert
            renderComponent
                .Find("a.levelOne")
                .MarkupMatches(@" <a class=""col-sm column levelOne"" href=""/products/4"">
                        <div class=""category-div"">
                            <h2>Costume</h2>
                            <p>Express your style</p>
                        </div>
                        <img src = ""/bag.png"" />
                    </ a >");
        }

        [Fact]
        public async Task HomePage_DefaultState_ExpectedLoginButton_InCategorySection_WithOutAuthrized()
        {
            // Arrange
            var stubProductServices = new ProductServiceMoq();
            Services.AddSingleton<IProdcutServices>(stubProductServices);

            var authContext = this.AddTestAuthorization();
            authContext.SetNotAuthorized();


            var renderComponent = RenderComponent<Home>();

            // Assert
            renderComponent
                .Find("a.levelOne")
                .MarkupMatches(@"<a class=""col-sm column levelOne"" href=""/login"">
                        <div class=""category-div"">
                            <h2>Costume</h2>
                            <p>Express your style</p>
                        </div>
                        <img src=""/bag.png"" />
                    </a>");
        }

    }









    //custome authentication state provider mocking
    public class MockCustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Simulate an authenticated user for testing
            var identity = new ClaimsIdentity(new List<Claim>
        {
            new Claim(ClaimTypes.Email, "usermail@gmail.com"),
            new Claim(ClaimTypes.Name, "usermail@gmail.com"),
        }, "mock");

            var user = new ClaimsPrincipal(identity);
            return Task.FromResult(new AuthenticationState(user));
        }
    }
}
