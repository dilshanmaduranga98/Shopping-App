using BlazorBootstrap;
using Microsoft.JSInterop;
using ShoppingApp.FrontEnd.Interfaces;
using System.Net;
using static ShoppingApp.FrontEnd.Pages.Login;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using ShoppingApp.FrontEnd.Models;
using Blazored.SessionStorage;
using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace ShoppingApp.FrontEnd.Services
{
    public class UserServices : IUserServices
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jSRuntime;
        private readonly ToastService _toastService;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ISessionStorageService _sessionStorageService;

        public UserServices(HttpClient httpClient, IJSRuntime jSRuntime, ToastService toastService, AuthenticationStateProvider authenticationStateProvider, ISessionStorageService sessionStorageService)
        {
            _httpClient = httpClient;
            _jSRuntime = jSRuntime;
            _toastService = toastService;
            _authenticationStateProvider = authenticationStateProvider;
            _sessionStorageService = sessionStorageService;
        }


        //user signin service
        /// <summary>
        /// Authenticates a user by posting login credentials to the server.
        /// </summary>
        /// <param name="loginData">The credentials used for authentication.</param>
        /// <returns>A <see cref="TokenModel"/> containing access and refresh tokens if successful; otherwise, null.</returns>
        public async Task<TokenModel> GetAccessToken(LoginModel loginData)
        {
            string resultMessage = string.Empty;
            try
            {
                // Post login credentials to the server
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7036/api/users/signin", loginData);
                Console.WriteLine(response);
                if (response.IsSuccessStatusCode)
                {
                    // Read the token response from the server
                    var responseToken = await response.Content.ReadFromJsonAsync<TokenModel>();

                    var accessToken = responseToken.Access_Token;
                    var refreshToken = responseToken.Refresh_Token;

                    var tokenResult = new TokenModel 
                    {
                        Access_Token = accessToken,
                        Refresh_Token = refreshToken,
                    };

                    if(tokenResult.Access_Token != null)
                    {
                        // Notify user of successful login
                        _toastService.Notify(new(ToastType.Success, $"{loginData.email} Login succesfully!"));
                    }else
                    {
                        // Notify user of unsuccessful login attempt
                        _toastService.Notify(new(ToastType.Danger, $"Login unsuccesfully!"));
                    }
                   
                    
                    Console.WriteLine($"Access Token : {accessToken}");

                    // Return the token model if tokens are not null
                    if (accessToken != null && refreshToken != null)
                    {
                        return tokenResult;
                    }else
                    {
                        return null;
                    }

                    

                }
                else
                {
                    resultMessage = "Login unsuccesfully!";

                    // Notify user of unsuccessful login attempt
                    _toastService.Notify(new(ToastType.Danger, $"Login unsuccesfully!"));
                    Console.WriteLine(resultMessage);
                    
                    return null;
                }

            }
            catch (Exception ex) 
            {
                // Handle exceptions during login process
                resultMessage = $"Login unsuccesfully!{ex.Message}";
                _toastService.Notify(new(ToastType.Danger, $"Login unsuccesfully!"));
                Console.WriteLine(resultMessage);
                throw new Exception(ex.Message);
            }
        }


        //user signup service
        /// <summary>
        /// Registers a new user by sending user information to the server.
        /// </summary>
        /// <param name="user">The user information to be registered.</param>
        /// <returns>True if user registration is successful; otherwise, false.</returns>
        public async Task<bool> RegisterUser(UserModel user)
        {
            try
            {
                bool userCreate = false;

                // Send user registration data to the server
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7036/api/users/signup", user);


                // Check if the server responds with OK status code
                if (response.StatusCode == HttpStatusCode.OK)
                {

                    userCreate = true;
                    Console.WriteLine(response);
                    
                    return userCreate;
                    
                }
                else
                {

                    userCreate = false;
                    return userCreate;
                    
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);

                // Handle exceptions during user registration
                throw new Exception(ex.Message);
                

            }
        }


        /// <summary>
        /// Retrieves user information from the server using the access token stored in session storage.
        /// </summary>
        /// <returns>A <see cref="UserModel"/> containing user information if retrieval is successful; otherwise, null.</returns>
        public async Task<UserModel> UserInfo()
        {
            try
            {
                // Retrieve access token from session storage
                var token = await _sessionStorageService.GetItemAsync<string>("access_Token");


                // Set authorization header with the retrieved access token
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                // Request user information from the server
                var result = await _httpClient.GetFromJsonAsync<UserModel>("https://localhost:7036/api/users/user-info");

                if(result != null)
                {
                    return result;
                }else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                // Handle exceptions during user information retrieval
                throw new Exception (ex.Message);
            }
        }


        /// <summary>
        /// Retrieves user's address information from the server using the access token stored in session storage.
        /// </summary>
        /// <returns>A list of <see cref="AddressModel"/> containing user's address information if retrieval is successful; otherwise, null.</returns>
        public async Task<List<AddressModel>> UserAddressInfo()
        {
            try
            {
                // Retrieve access token from session storage
                var token = await _sessionStorageService.GetItemAsync<string>("access_Token");


                // Set authorization header with the retrieved access token
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                // Request user's address information from the server
                var result = await _httpClient.GetFromJsonAsync<List<AddressModel>>("https://localhost:7036/api/users/address");

                if (result != null)
                {
                    return result;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {

                // Handle exceptions during user's address information retrieval
                throw new Exception(ex.Message);
            }
        }
    }
}
