using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using ShoppingCart.Application.DTOs;
using ShoppingCart.Application.Interfaces;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using static SendGrid.BaseClient;
using System.Text.RegularExpressions;
using ShoppingCart.Domain.Helper;
using ShoppingCart.Infrastructure.Data;
using ShoppingCart.Infrastructure.Services;
using ShoppingCart.Domain.Models;
using ShoppingCart.Application.Interfaces.IRepositories;
using System.Net;
using static System.Formats.Asn1.AsnWriter;


namespace ShoppingCart.Infrastructure.AuthServicec
{

    // Service responsible for authentication operations
    public class AuthService : IAuthService
    {
        private readonly KeyConfigurations _authConfigurations;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;
        private readonly IEmailInterface _emailInterface;
        private  HttpClient _httpClient;



        // Constructor for AuthService, injects AuthConfigurations via IOptions
        public AuthService(IOptions<KeyConfigurations> options,
                           IPaymentService paymentService,
                           IEmailInterface emailInterface,
                           IUnitOfWork unitOfWork)
        {
            _authConfigurations = options.Value;
            _paymentService = paymentService;
            _emailInterface = emailInterface;
            _unitOfWork = unitOfWork;
            _httpClient = new HttpClient();

        }

      
        // Method to handle user signup process
        public async Task<string> userSignup(UserDTO userReq)
        {

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var password = userReq.password;
                var confirm_password = userReq.confirmPassword;

                if (password != confirm_password)
                {
                    Log.Error("Password and confirm passwords are not match!");

                    return "password and confirm password are note match!";
                }

                // Prepare payload for signup request
                var payload = new
                {
                    client_id = _authConfigurations.ClientId,
                    email = userReq.email,
                    password = userReq.password,
                    connection = "Username-Password-Authentication"
                };



                var jsonPayload = JObject.FromObject(payload).ToString();
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");


                // Send signup request to Auth0
                //var client = _httpClient;
                var response = await _httpClient.PostAsync("https://dev-j510p5iw1blv70i3.us.auth0.com/dbconnections/signup", content);

                // Check if signup was successful
                if (response.IsSuccessStatusCode)
                {
                    var getString = await response.Content.ReadAsStringAsync();
                    TokenID toJson = JsonConvert.DeserializeObject<TokenID>(getString);
                    var user_id = toJson._id;



                    // Get stripe customer id
                    var customerID = await _paymentService.CreateCustomer(userReq.firstName, userReq.email);


                    // Create new user object
                    var newUser = new User
                    {
                        userID = user_id,
                        firstname = userReq.firstName,
                        lastname = userReq.LastName,
                        email = userReq.email,
                        phonenumber = userReq.phoneNumber,
                        strip_CustomerID = customerID.Id
                    };

                    var userAdd = await _unitOfWork.GetBaseRepository<User>().AddAsync(newUser);
                    await _unitOfWork.SaveChangesAsync();



                    // Create new address object
                    var newAddress = new Address
                    {
                        userID = user_id,
                        street = userReq.addressDTO.street,
                        city = userReq.addressDTO.city,
                        country = userReq.addressDTO.country,
                        postalCode = userReq.addressDTO.postalCode,
                        IsPrimary = true,
                    };

                    // Add new address and user to database
                    var addAddress = await _unitOfWork.GetBaseRepository<Address>().AddAsync(newAddress);
                    await _unitOfWork.SaveChangesAsync();

                    await _unitOfWork.CommitAsync();


                    // Send welcome email to user
                    var htmlContent = EmailContectHelper.CreateWelcomeEmail(newUser);
                    var sublect = $"Welcome {userReq.firstName}!";
                    var palintText = "Thank you for choosing us for your shopping needs.";

                    await _emailInterface.sendGridService(userReq.email, userReq.firstName, htmlContent, sublect, palintText);


                    Log.Information("New user create succesfully, user ID => {@userID}", user_id);


                    return "User create succefully!";
                }
                else
                {
                    return $"Response not give sucess code 200!";
                }

            }catch 
            {
                await _unitOfWork.RollbackAsync();
                throw new("Error occurred suring try to register the user!");
            }
        }


        // Method to handle user login process
        public async Task<AccessTokenDTO> userLogin(string email, string password)
        {
            var Email = email;
            var Password = password;
            var audience = _authConfigurations.Audience;
            var request_body = $"grant_type=password&username={Email}&password={Password}&client_id={_authConfigurations.ClientId}&client_secret={_authConfigurations.ClientSecret}&audience={audience}&scope=openid profile email offline_access";
            var content = new StringContent(request_body, Encoding.UTF8, "application/x-www-form-urlencoded");

            // Send login request to Auth0
           
            var response = await _httpClient.PostAsync("https://dev-j510p5iw1blv70i3.us.auth0.com/oauth/token", content);


            // Check if login was successful
            if (response.StatusCode== HttpStatusCode.OK)
            {
                var tokenResponse = await response.Content.ReadAsStringAsync();
                AccessTokenDTO tokenToJson = JsonConvert.DeserializeObject<AccessTokenDTO>(tokenResponse);
                var accessToken = tokenToJson.access_token;
                var refreshtoken = tokenToJson.refresh_token;

                var result = new AccessTokenDTO
                {
                    access_token = accessToken,
                    refresh_token = refreshtoken,
                };
                return result;

            }
            else
            {
                return new AccessTokenDTO();
            }
              

        }

        public void SetHttpClient(HttpClient client)
        {
            _httpClient = client;
        }




        //get access_token from refresh token
        public async Task<AccessTokenDTO> GetAccessTokenFromRefreshToken(string token)
        {
            var request = $"grant_type=refresh_token&client_id={_authConfigurations.ClientId}&client_secret={_authConfigurations.ClientSecret}&refresh_token={token}&scope=openid%20profile";
            var content = new StringContent(request, Encoding.UTF8, "application/x-www-form-urlencoded");

            var response =await _httpClient.PostAsync($"https://dev-j510p5iw1blv70i3.us.auth0.com/oauth/token", content);
           

            //Check if login was successful
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var tokenResponse = await response.Content.ReadAsStringAsync();
                AccessTokenDTO tokenToJson = JsonConvert.DeserializeObject<AccessTokenDTO>(tokenResponse);
                var accessToken = tokenToJson.access_token;
                var refreshtoken = tokenToJson.refresh_token;

                var result = new AccessTokenDTO
                {
                    access_token = accessToken,
                    refresh_token = "",
                };



                return result;

            }
            else
            {
                return new AccessTokenDTO();
            }
        }

    }
}



