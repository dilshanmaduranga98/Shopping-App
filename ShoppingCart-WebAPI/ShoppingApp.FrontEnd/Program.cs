using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ShoppingApp.FrontEnd;
using ShoppingApp.FrontEnd.Authentication;
using ShoppingApp.FrontEnd.Interfaces;
using ShoppingApp.FrontEnd.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddBlazorBootstrap();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthenticationStateProvider,CustomAuthenticationStateProvider>();
builder.Services.AddScoped<IUserServices,UserServices>();
builder.Services.AddScoped<IProdcutServices,ProductServices>();
builder.Services.AddScoped<IOrderServices, OrderServices>();

// Register the DefaultAuthorizationPolicyProvider
builder.Services.AddSingleton<IAuthorizationPolicyProvider, DefaultAuthorizationPolicyProvider>();


await builder.Build().RunAsync();
