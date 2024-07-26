using Bunit;
using Moq;
using ShoppingApp.FrontEnd.Interfaces;
using ShoppingApp.FrontEnd.Models;
using ShoppingApp.FrontEnd.Pages;
using Microsoft.Extensions.DependencyInjection;
using ShoppingApp.FrontEnd.Test.StubMoqServices;
using Microsoft.AspNetCore.Authorization;
using Blazored.LocalStorage;
using ShoppingApp.FrontEnd.Services;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Bunit.TestDoubles;
using FluentAssertions;
using BlazorBootstrap;

namespace ShoppingApp.FrontEnd.Test.Page_Testing
{
    public class ProductPageTest:TestContext
    {
        
        public ProductPageTest()
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
        public async Task ProductsPage_Render_AllProducts()
        {
            // Arrange
            var authContext = this.AddTestAuthorization();
            authContext.SetAuthorized("TestUser");


            var component = RenderComponent<Products>(parameters =>
                parameters.Add(p => p.categoryID, 0));
            

            // Act
            

            // Assert
            component.Find(".product-title-container").MarkupMatches(@" <div class=""product-title-container"">
                        <h1 class=""main-header"">All Products</h1>
                        <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit</p>
                    </div>");
            component.FindAll(".item-container .col").Count.Should().Be(2); // Adjust count based on actual products returned
        }


        [Fact]
        public async Task ProductsPage_Render_ByCategory()
        {
            // Arrange
            var authContext = this.AddTestAuthorization();
            authContext.SetAuthorized("TestUser");

            var component = RenderComponent<Products>(parameters =>
                parameters.Add(p => p.categoryID, 1) // Adjust category ID as needed
            );

            // Act

            // Assert
            component.Find(".product-title-container")
                .MarkupMatches(@"<div class=""product-title-container"">
                        <h1 class=""main-header"">Category 1</h1>
                        <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit</p>
                    </div>");
            component.FindAll(".item-container .col").Count.Should().Be(2); // Adjust count based on mockProducts list
        }


        [Fact]
        public async Task ProductsPage_NoProductsFound()
        {
            // Arrange

            var productServiceMock = new Mock<IProdcutServices>();
            productServiceMock.Setup(service => service.GetAllProducts())
                              .ReturnsAsync((List<ProductModel>)null);

            var authContext = this.AddTestAuthorization();
            authContext.SetAuthorized("TestUser");

            using var ctx = new TestContext();
            ctx.Services.AddAuthorizationCore();
            ctx.Services.AddSingleton<IAuthorizationPolicyProvider, DefaultAuthorizationPolicyProvider>();
            ctx.Services.AddBlazorBootstrap();
            ctx.Services.AddBlazoredLocalStorage();
            ctx.Services.AddBlazoredSessionStorage();
            ctx.Services.AddScoped<IOrderServices, OrderServices>();
            ctx.Services.AddSingleton<IProdcutServices>(productServiceMock.Object);
            ctx.Services.AddScoped<AuthenticationStateProvider, MockCustomAuthenticationStateProvider>();

            var component = ctx.RenderComponent<Products>(); // Adjust category ID as needed

            // Act

            // Assert
           
            component.FindAll(".item-container .col").Count.Should().Be(0); // Adjust count based on mockProducts list
        }


        [Fact]
        public async Task ProductsPage_GetProductsNull_Spinners_ShouldRender()
        {
            // Arrange

            var productServiceMock = new Mock<IProdcutServices>();
            productServiceMock.Setup(service => service.GetAllProducts())
                              .ReturnsAsync((List<ProductModel>)null);

            var authContext = this.AddTestAuthorization();
            authContext.SetAuthorized("TestUser");

            using var ctx = new TestContext();
            ctx.Services.AddAuthorizationCore();
            ctx.Services.AddSingleton<IAuthorizationPolicyProvider, DefaultAuthorizationPolicyProvider>();
            ctx.Services.AddBlazorBootstrap();
            ctx.Services.AddBlazoredLocalStorage();
            ctx.Services.AddBlazoredSessionStorage();
            ctx.Services.AddScoped<IOrderServices, OrderServices>();
            ctx.Services.AddSingleton<IProdcutServices>(productServiceMock.Object);
            ctx.Services.AddScoped<AuthenticationStateProvider, MockCustomAuthenticationStateProvider>();

            var component = ctx.RenderComponent<Products>(parameters =>
                parameters.Add(p => p.categoryID, 0)); // Adjust category ID as needed

            // Act

            // Assert
            //component.FindAll(".col")
            //    .MarkupMatches(@"<div class=""col"" style=""display:flex; width:100%; justify-content:center; align-items:center;"">
                               
            //                    <Spinner Type = ""SpinnerType.Dots"" Class=""me-3"" Color=""SpinnerColor.Warning"" Size=""SpinnerSize.ExtraLarge"" />
            //            </div>");

            component.FindAll(".item-container .col").Count.Should().Be(0);

        }


        
    }



    
}
