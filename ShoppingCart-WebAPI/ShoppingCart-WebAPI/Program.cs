using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SendGrid.Helpers.Mail;
using Serilog;
using ShoppingCart.Infrastructure.AuthServicec;
using ShoppingCart.Infrastructure.Data;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Application.Services.Implementation;
using System.Reflection;
using System.Security.Claims;
using ShoppingCart.Application.Services.Interfaces;
using ShoppingCart.Application.Interfaces.IRepositories;
using ShoppingCart.Infrastructure.Repositories;
using ShoppingCart.Domain.Models;
using ShoppingCart.Infrastructure.Services;
using ShoppingCart.Infrastructure.CustomMiddleware;

// Initialize the WebApplication builder
var builder = WebApplication.CreateBuilder(args);


// Configure JSON serialization options for controllers
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( option =>
{
    // Include XML comments for Swagger documentation
    option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});



//****************************************************************************************************************
builder.Services.AddProblemDetails(); // add for use IExceptionHandller
//****************************************************************************************************************



// Add database context
builder.Services.AddDbContext<ShoppingDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("conString")));





builder.Services.AddHttpContextAccessor();     // Add HttpContextAccessor for accessing HTTP context in services

builder.Services.AddScoped<IProductService, ProductService>();      // Register ProductService implementation for IProductService interface

builder.Services.AddScoped<IAddressService, AddressService>();          // Register AddressService implementation for IAddressService interface

builder.Services.AddScoped<ITokenServices, TokenService>();     // Register TokenService implementation for ITokenServices interface

builder.Services.AddScoped<IOrderServices, OrderServices>();        // Register OrderServices implementation for IOrderServices interface

builder.Services.AddScoped<IPaymentService, PaymentService>();      // Register PaymentService implementation for IPaymentService interface

builder.Services.AddScoped<IEmailInterface, EmailService>();            // Register EmailService implementation for IEmailInterface interface

builder.Services.AddScoped<IAuthService, AuthService>();        // Register AuthService implementation for IAuthService interface

builder.Services.AddScoped<IUpdateTimeStampService, UpdateTimeStampService>();        // Register AuthService implementation for IAuthService interface

builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRopository<>));   // Register BaseRepository implementation for IBaseRepository interface

builder.Services.AddScoped<IUnitOfWork, UnitOfWork> (); // Register UnitOfWork implementation for IUnitOfWork interface




builder.Services.AddHttpClient();

//Swagger Authorize button 
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.Configure<KeyConfigurations>(options =>
{
    options.ClientId = builder.Configuration["Auth0:ClientId"];
    options.Domain = builder.Configuration["Auth0:Domain"];
    options.Audience = builder.Configuration["Auth0:Audience"];
    options.ClientSecret = builder.Configuration["Auth0:ClientSecret"];
    options.StripeKey = builder.Configuration["Stripe:SecretKey-Stripe"];
    options.SendGridKey = builder.Configuration["SendGrid:SecretKey-SendGrid"];

});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
      options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
      options.Audience = builder.Configuration["Auth0:Audience"];
      options.TokenValidationParameters = new TokenValidationParameters
      {
          NameClaimType = ClaimTypes.NameIdentifier
      };
  });

builder.Services.AddAuthorization();


//seriLog configuration
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/shoppinAppLogs-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();



var app = builder.Build();

//enable CORS
app.UseCors(c => 
c.AllowAnyHeader()
.AllowAnyMethod()
.SetIsOriginAllowed(origin => true)
.AllowCredentials());   //it allows credentials to be sent , which is necessary if your application requires authentication tokens or cookies to be included in cross-origin requests.



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthentication(); //middleware

app.UseAuthorization(); //middleware

app.UseMiddleware<GlobalCustomMiddleware>();  // custome middleware for exception handling

app.MapControllers();


app.Run();
