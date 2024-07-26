using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using ShoppingApp.FrontEnd.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace ShoppingApp.FrontEnd.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {

        private readonly ISessionStorageService _sessionStorage;   // Service for accessing session storage
        private readonly IHttpContextAccessor _contextAccessor;   
        private readonly HttpClient _httpClient;     // HTTP client for making API requests
        private readonly IJSRuntime _jsruntime;
        private readonly NavigationManager _navigationManager;    // Manages navigation within the application
        private ClaimsPrincipal _principal = new ClaimsPrincipal(new ClaimsIdentity());    // Stores the authenticated user's claims

        private readonly string ClientId = "iAVfYaKw6Q7FhDnm73A5KY14Tuz2mvVJ";
        private readonly string ClientSecret = "JvYt4xzrKimiozD9m9zZcac4zDA_nmuazP7-KysrZMLtbm65tQ_gBtXCVzb8mKE1";

        public CustomAuthenticationStateProvider( ISessionStorageService sessionStorge, HttpClient httpClient, NavigationManager navigationManager)
        {

            _sessionStorage = sessionStorge;
            _httpClient = httpClient;
            _navigationManager = navigationManager;

        }

        // Gets the current authentication state asynchronously
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                

                ClaimsPrincipal claimsPrincipal;

                // Retrieve tokens and user email from session storage
                var userSessionStorageResult = await _sessionStorage.GetItemAsync<string>("access_Token");
                var userEmailResult = await _sessionStorage.GetItemAsync<string>("email");
                var userRefreshToken = await _sessionStorage.GetItemAsync<string>("refresh_Token");


                // Check if token is expired or not present
                if (IsTokenExpired(userSessionStorageResult) || string.IsNullOrWhiteSpace(userSessionStorageResult) )
                {

                    // If token is expired, attempt to refresh it
                    if (IsTokenExpired(userSessionStorageResult))
                    {
                            var request = $"grant_type=refresh_token&client_id={ClientId}&client_secret={ClientSecret}&refresh_token={userRefreshToken}&scope=openid%20profile";
                            var content = new StringContent(request, Encoding.UTF8, "application/x-www-form-urlencoded");
                            var response = await _httpClient.PostAsync("https://dev-j510p5iw1blv70i3.us.auth0.com/oauth/token", content);


                        // Handle successful refresh token request
                        if (response.StatusCode == HttpStatusCode.OK)
                            {
                                var tokenResponse = await response.Content.ReadAsStringAsync();
                                TokenModel tokenToJson = JsonConvert.DeserializeObject<TokenModel>(tokenResponse);
                                var accessToken = tokenToJson.Access_Token;
                                var refreshtoken = tokenToJson.Refresh_Token;

                                var result = new TokenModel
                                {
                                    Access_Token = accessToken,
                                    Refresh_Token = refreshtoken,
                                };


                                // Store refreshed tokens in session storage
                                await _sessionStorage.SetItemAsync("access_Token", accessToken);
                                await _sessionStorage.SetItemAsync("refresh_Token", refreshtoken);


                            // Create new claims principal with refreshed tokens and user information
                            claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                                {
                                    new Claim(ClaimTypes.Email, accessToken),
                                    new Claim(ClaimTypes.Authentication, refreshtoken),
                                    new Claim(ClaimTypes.Name, userEmailResult),
                                }));

                             }
                            else
                            {
                                // Return unauthenticated state if refresh token request fails
                                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                            }


                       

                    }

                    // Return unauthenticated state if token is expired or not present
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }
                else
                {
                    // Return authenticated state with stored user claims
                    _principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, userSessionStorageResult),
                        new Claim(ClaimTypes.Name, userEmailResult)

                    }, "CustomAuth"));  //change ponit "CustomAuth"
                    return await Task.FromResult(new AuthenticationState(_principal));

                }
            }catch 
            {
                // Return unauthenticated state if an exception occurs
                return await Task.FromResult(new AuthenticationState(_principal));
            }
            
        }


        // Updates the authentication state with new tokens and user information
        public async Task UpdateAuthenticationState(TokenModel token, string email)
        {
            ClaimsPrincipal claimsPrincipal;


            // Update authentication state with new tokens if available
            if (token != null)
            {
                // Store new tokens and user email in session storage
                await _sessionStorage.SetItemAsync("access_Token", token.Access_Token);
                await _sessionStorage.SetItemAsync("refresh_Token", token.Refresh_Token);
                await _sessionStorage.SetItemAsync("email", email);

                // Create new claims principal with updated tokens and user information
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Email, token.Access_Token),
                    new Claim(ClaimTypes.Authentication, token.Refresh_Token),
                    new Claim(ClaimTypes.Name, email),
                }));

            }else
            {

                // Remove tokens and email from session storage and revert to previous principal
                await _sessionStorage.RemoveItemAsync("access_Token");
                await _sessionStorage.RemoveItemAsync("refresh_Token");
                await _sessionStorage.RemoveItemAsync("email");
                claimsPrincipal = _principal;
            }

            // Notify authentication state change to update UI
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }



        //-------------------------------------------------------------------------------

        // Checks if the provided token is expired
        private bool IsTokenExpired(string token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadToken(token) as JwtSecurityToken;


            // Check if JWT token or its expiration claim is null
            if (jwtToken == null)
                return true;

            var exp = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;

            // Check if expiration claim value is null or token is expired
            if (exp == null)
                return true;

            var expDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp));

             // Compare expiration date with current UTC time to determine token expiry
            return expDate < DateTimeOffset.UtcNow;
        }
        //-------------------------------------------------------------------------------
    }
}
